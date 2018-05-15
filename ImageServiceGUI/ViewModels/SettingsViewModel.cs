using ImageServiceGUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel :INotifyPropertyChanged
    {
        private ISettingsModel settingsModel;
        public SettingsViewModel()
        {
            settingsModel =new SettingsModel();
            settingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
              {
                  NotifyPropertyChanged("VM_" + e.PropertyName);
              };
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
    }
}
