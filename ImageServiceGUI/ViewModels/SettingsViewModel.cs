using ImageServiceGUI.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel :INotifyPropertyChanged
    {
        private ISettingsModel settingsModel;
        public ICommand RemoveCommand { get; private set; }
        
        public SettingsViewModel()
        {
            settingsModel =new SettingsModel();
            settingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
              {
                  NotifyPropertyChanged("VM_" + e.PropertyName);
              };

            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        //Properties
        public string VM_OutputDir
        {
            get { return settingsModel.OutputDir; }
        }

        public string VM_SourceName
        {
            get { return settingsModel.SourceName; }
        }

        public string VM_LogName
        {
            get { return settingsModel.LogName; }
        }

        public int VM_ThumbnailSize
        {
            get { return settingsModel.ThumbnailSize; }
        }

        public ObservableCollection<string> VM_Handlers
        {
            get { return settingsModel.Handlers; }
        }

        private string selectedHandler;
        public string SelectedHandler
        {
            get { return selectedHandler; }
            set
            {
                selectedHandler = value;
                if(RemoveCommand as DelegateCommand<object> !=null)
                    (RemoveCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            }
        }
        
        private bool CanRemove(object obj)
        {
            if (SelectedHandler != null)
                return true;
            else
                return false;
        }


        private void OnRemove(object obj)
        {
            //SelectedHandler = null;
        }
        
    }
}
