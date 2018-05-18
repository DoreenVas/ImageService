using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class LogModel : ILogModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public LogModel()
        {
            Logs = new ObservableCollection<MessageRecievedEventArgs>();
            MessageRecievedEventArgs a = new MessageRecievedEventArgs() { Status = MessageTypeEnum.WARNING, Message="IWIIIII" };
            Logs.Add(a);
            MessageRecievedEventArgs b = new MessageRecievedEventArgs() { Status = MessageTypeEnum.INFO, Message = "OMIIIIII" };
            Logs.Add(b);
            MessageRecievedEventArgs c = new MessageRecievedEventArgs() { Status = MessageTypeEnum.FAIL, Message = "yayaaasss" };
            Logs.Add(c);
            
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
       
    }
}
