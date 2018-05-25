using ImageService.Communication.Client;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        public IClientCommunicationChannel Client { get; set; }
        public SettingsModel()
        {
            Client = TcpClientChannel.Instance;
            Client.RecieveCommand();
            Client.ServerCommandRecieved += ServerCommandRecieved;
            Initializer();  
        }
        private void Initializer()
        {
            Handlers = new ObservableCollection<string>();
            string[] arr = new string[1];
            CommandRecievedEventArgs request = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, arr, "");
            Client.SendCommand(request);
        }

        private void ServerCommandRecieved(CommandRecievedEventArgs commandRead)
        {
                if (commandRead != null && commandRead.CommandID == (int)CommandEnum.GetConfigCommand)
                {
                    OutputDir = commandRead.Args[0];
                    SourceName = commandRead.Args[1];
                    LogName = commandRead.Args[2];
                    ThumbnailSize = commandRead.Args[3];
                    Object thisLock = new Object();
                    BindingOperations.EnableCollectionSynchronization(Handlers, thisLock);
                    string[] handlers = commandRead.Args[4].Split(';');
                    foreach (string handler in handlers)
                    {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Handlers.Add(handler);
                    });
                    }
                }
                else if (commandRead != null && commandRead.CommandID == (int)CommandEnum.CloseCommand)
                {
                    if (Handlers.Contains(commandRead.Args[0]))
                    {
                        Handlers.Remove(commandRead.Args[0]);
                        NotifyPropertyChanged("Handlers");
                    }
                }
            
        }

        //the properties implementation
        private string outputDir;
        public string OutputDir
        {
            get { return outputDir; }
            set {
                outputDir = value;
                NotifyPropertyChanged("OutputDir");
            }
        }

        private string sourceName;
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                sourceName = value;
                NotifyPropertyChanged("SourceName");
            }
        }

        private string logName;
        public string LogName
        {
            get { return logName; }
            set
            {
                logName = value;
                NotifyPropertyChanged("LogName");
            }
        }

        private string thumbnailSize;
        public string ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        public ObservableCollection<string> Handlers { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
