using ImageService.Communication.Client;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using Newtonsoft.Json;
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
    class LogModel : ILogModel
    {
        private bool canAcceptLogs = false;
        public IClientCommunicationChannel Client { get; set; }
        public LogModel()
        {
            Client = TcpClientChannel.Instance;
            //Client.RecieveCommand();
            Client.ServerCommandRecieved += ServerCommandRecieved;
            Initializer();
            
        }
        private void Initializer()
        {
            Logs = new ObservableCollection<MessageRecievedEventArgs>();
            CommandRecievedEventArgs request = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, null, "");
            Client.SendCommand(request);
        }

        private void ServerCommandRecieved(CommandRecievedEventArgs commandRead)
        {
            if (commandRead != null && commandRead.CommandID == (int)CommandEnum.LogCommand)
            { 
                Object thisLock = new Object();
                BindingOperations.EnableCollectionSynchronization(Logs, thisLock);
                List<MessageRecievedEventArgs> recievedLogs = JsonConvert.DeserializeObject<List<MessageRecievedEventArgs>>(commandRead.Args[0]); 
                foreach (MessageRecievedEventArgs log in recievedLogs)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Logs.Add(log);
                    });
                }
                canAcceptLogs = true;
            }
            else if (commandRead != null && commandRead.CommandID == (int)CommandEnum.NewLogCommand && Logs!=null && canAcceptLogs==true)
            {
                Object thisLock = new Object();
                BindingOperations.EnableCollectionSynchronization(Logs, thisLock);
                MessageRecievedEventArgs recievedLog = JsonConvert.DeserializeObject<MessageRecievedEventArgs>(commandRead.Args[0]);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Logs.Add(recievedLog);
                });
            }
        }

        private ObservableCollection<MessageRecievedEventArgs> logs;
        public ObservableCollection<MessageRecievedEventArgs> Logs {
            get { return logs; }
            set
            {
                logs = value;
                NotifyPropertyChanged("Logs");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
