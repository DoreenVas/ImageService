using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    interface ISettingsModel :INotifyPropertyChanged
    {
        //properties
        string OutputDir { get; set; }
        string SourceName { get; set; }
        string LogName { get; set; }
        int ThumbnailSize { get; set; }
        ObservableCollection<string> Handlers { get; set; }
    }
}
