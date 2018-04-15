using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        private readonly string[] filters = { ".jpg",".JPG", ".png", ".PNG", ".gif", ".GIF", ".bmp", ".BMP" };

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose; // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController m_controller, ILoggingService m_logging)
        {
            this.m_controller = m_controller;
            this.m_logging = m_logging;
           
        }

        public void StartHandleDirectory(string dirPath) // The Function Recieves the directory to Handle
        {
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher(m_path);
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);// Add event handler
            m_dirWatcher.EnableRaisingEvents = true;  // Begin watching.
            m_logging.Log("Started handling the directory in path: " + m_path, MessageTypeEnum.INFO);
        }

        private void OnCreated(object source, FileSystemEventArgs e)// new image recieved
        {
            string extension = Path.GetExtension(e.FullPath);
            if (filters.Contains(extension))
            {
                bool result=WaitForFullFile(new FileInfo(e.FullPath));//waiting for the file to be completly copied before handling it
                if(result == false)
                {
                    m_logging.Log("Unable to move image: " + e.FullPath + " due to file creation timeout", MessageTypeEnum.FAIL);
                    return;
                }
                int commandId = (int)CommandEnum.NewFileCommand;
                string[] args = { e.FullPath };
                CommandRecievedEventArgs commandArgs = new CommandRecievedEventArgs(commandId, args, m_path);
                OnCommandRecieved(this, commandArgs);
            }
            else
            {
                m_logging.Log("Invalid extension: "+extension+" detected while listening to directory "+m_path , MessageTypeEnum.FAIL);
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) // The Event that will be activated upon new Command
        {   //checking if our path is the one the server intended to handle the command
            if (e.RequestDirPath.Equals(m_path) || e.RequestDirPath.Equals("*"))// Equals compares content
            {
                // if CloseCommand
                if (e.CommandID == (int)CommandEnum.CloseCommand)// == better for numbers
                {
                    m_dirWatcher.EnableRaisingEvents = false;  // stop watching.
                    string exitMsg = "Stoped handling the directory in path: " + m_path;
                    DirectoryCloseEventArgs directoryCloseEventArgs = new DirectoryCloseEventArgs(m_path, exitMsg);
                    DirectoryClose?.Invoke(this, directoryCloseEventArgs);
                }
                else // if other command
                {
                    bool success;
                    string msg = m_controller.ExecuteCommand(e.CommandID, e.Args, out success);
                    if (success)
                    {
                        m_logging.Log(msg, MessageTypeEnum.INFO);
                    }
                    else
                    {
                        m_logging.Log(msg, MessageTypeEnum.FAIL);
                    }
                }
            }
        }

        private bool WaitForFullFile(FileInfo file)
        {
            const int timeMax = 20000;
            int time=0;
            while (IsFileLocked(file) && time<=timeMax)
            {
                Thread.Sleep(100);
                time += 100;
            }
            if(time>timeMax && IsFileLocked(file))
            {
                return false;
            }
            return true;
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
