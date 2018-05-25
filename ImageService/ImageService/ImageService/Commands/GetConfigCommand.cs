using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class GetConfigCommand : ICommand
    {
        public string Execute(string[] args, out bool success)
        {
            try
            {
                string outputDir = ConfigurationManager.AppSettings.Get("OutputDir");
                string thumbnailSize = ConfigurationManager.AppSettings.Get("ThumbnailSize");
                string eventSourceName = ConfigurationManager.AppSettings.Get("SourceName");
                string logName = ConfigurationManager.AppSettings.Get("LogName");

                string[] arr = new string[5];
                arr[0] = outputDir;
                arr[1] = eventSourceName;
                arr[2] = logName;
                arr[3] = thumbnailSize;
                arr[4] = args[0];
                CommandRecievedEventArgs commandSend = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, arr, "");
                success = true;
                return JsonConvert.SerializeObject(commandSend);
            }
            catch (Exception exc)
            {
                success = false;
                return exc.ToString();
            }
        }
    }
}
