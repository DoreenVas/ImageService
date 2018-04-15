using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>();

            commands[(int)CommandEnum.NewFileCommand] = new NewFileCommand(m_modal);     
        }

        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            Task <Tuple<string, bool>> command = new Task <Tuple<string, bool>>(() =>
              {
                  bool resultSuccesfulCheck;
                  string msg= commands[commandID].Execute(args, out resultSuccesfulCheck);
                  return Tuple.Create(msg, resultSuccesfulCheck);
              });
            command.Start();
            command.Wait();
            Tuple<string, bool> result = command.Result;
            resultSuccesful = result.Item2;
            return result.Item1;

        }
    }
}
