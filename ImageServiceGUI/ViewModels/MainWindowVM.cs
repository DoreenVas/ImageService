using ImageServiceGUI.Models;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageServiceGUI.ViewModels
{
    class MainWindowVM : INotifyPropertyChanged
    {
        private IMainWindowModel mainWindowModel;
        public ICommand CloseCommand { get; private set; }

        public MainWindowVM()
        {
            mainWindowModel = new MainWindowModel();
            mainWindowModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
            CloseCommand = new DelegateCommand<object>(OnClose, CanClose);
        }

        public bool VM_Connected
        {
            get { return mainWindowModel.Connected; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private bool CanClose(object arg)
        {
            return true;
        }

        private void OnClose(object obj)
        {
            mainWindowModel.Client.CloseClient();
        }
    }
}
