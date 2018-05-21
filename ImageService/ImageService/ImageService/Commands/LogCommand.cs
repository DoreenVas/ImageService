using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        private ILoggingService m_logging;
        public LogCommand(ILoggingService logging)
        {
            m_logging = logging;      
        }

        public string Execute(string[] args, out bool result)
        {
            try
            {
                List<MessageRecievedEventArgs> logsList = m_logging.LogList;
                string jsonLogsList= JsonConvert.SerializeObject(logsList);
                string[] arr = new string[1];
                arr[0] = jsonLogsList;
                CommandRecievedEventArgs commandSend = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, arr, "");
                result = true;
                return JsonConvert.SerializeObject(commandSend);
            }
            catch (Exception exc)
            {
                result = false;
                return exc.ToString();
            }
        }
    }
}
