using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {    
        private IImageController m_controller;
        private ILoggingService m_logging;
        private string m_handler;
        
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved; // The event that notifies about a new Command being recieved

        public ImageServer(IImageController imageController, ILoggingService loggingService, string handler)
        {
            m_controller = imageController;
            m_logging = loggingService;
            m_handler = handler;

            string[] paths = m_handler.Split(';');
            foreach (var path in paths)
            {
                CreateDirectoryHandler(path);
            }
        }
        
        private void CreateDirectoryHandler(string path)
        {
            IDirectoryHandler directoryHandler = new DirectoyHandler(m_controller, m_logging);
            m_logging.Log("A directory handler was created for the directory in path: " + path, Logging.Modal.MessageTypeEnum.INFO);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += this.OnDirectoryClose;
            directoryHandler.StartHandleDirectory(path);
        }

        private void OnDirectoryClose(object sender, DirectoryCloseEventArgs args)
        {
            if (sender is IDirectoryHandler)
            {
                IDirectoryHandler directoryHandler = (IDirectoryHandler)sender;
                CommandRecieved -= directoryHandler.OnCommandRecieved;
                m_logging.Log(args.Message, Logging.Modal.MessageTypeEnum.INFO);
            }
        }

        public void CloseServer()
        {
            string[] args = { };
            CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, args, "*");
            CommandRecieved?.Invoke(this, e);
            m_logging.Log("Server is Closing ", Logging.Modal.MessageTypeEnum.INFO);
        }
    }
}
