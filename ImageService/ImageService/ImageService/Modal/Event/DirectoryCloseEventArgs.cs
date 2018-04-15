using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    /// <summary>
    /// Represents a directory close event arguments.
    /// </summary>
    public class DirectoryCloseEventArgs : EventArgs
    {
        
        public string DirectoryPath { get; set; }       // The path of the directory to close.

        public string Message { get; set; }             // The Message That goes to the logger

        /// <summary>
        /// Constructor. creates a directory close event arguments instance.
        /// </summary>
        /// <param name="dirPath"></param> The path of the directory to close.
        /// <param name="message"></param> The logger's message.
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                    // Setting the Directory Name
            Message = message;                          // Storing the String
        }

    }
}
