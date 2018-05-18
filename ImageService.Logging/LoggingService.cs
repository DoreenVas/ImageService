
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    /// <summary>
    /// LoggingService class
    /// </summary>
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// invokes the MessageRecieved event with the correct arguments 
        /// </summary>
        /// <param name="message">message to write</param>
        /// <param name="type">type of message</param>
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs e = new MessageRecievedEventArgs();
            e.Message = message;
            e.Status = type;
            MessageRecieved?.Invoke(this, e);
        }
    }
}
