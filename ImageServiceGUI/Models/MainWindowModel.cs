using ImageService.Communication.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class MainWindowModel : IMainWindowModel
    {
        public IClientCommunicationChannel Client { get; set; }

        public MainWindowModel()
        {
            Client = TcpClientChannel.Instance;
            Connected = Client.IsConnected;
        }

        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                NotifyPropertyChanged("Connected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
