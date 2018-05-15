using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public SettingsModel()
        {
            OutputDir = "where to output dir";
            SourceName = "where from source name";
            LogName = "my log name";
            ThumbnailSize = 120;
            Handlers = new ObservableCollection<string>();
            Handlers.Add("first one");
            Handlers.Add("second one");
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

        private int thumbnailSize;
        public int ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        public ObservableCollection<string> Handlers { get; set; }

    }
}
