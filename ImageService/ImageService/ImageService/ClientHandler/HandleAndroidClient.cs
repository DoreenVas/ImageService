using ImageService.Communication.Server;
using ImageService.Controller;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ClientHandler
{
    class HandleAndroidClient : IClientHandler
    {
        public event NotifyClients NotifyClients;
        public object getLock() { return new object(); }
        private IImageController m_controller;
        private ILoggingService m_logging;
        private ImageServer m_imageServer;
        private const int c_sizeBytesCount = 4;

        public HandleAndroidClient(IImageController imageController, ILoggingService loggingService, ImageServer imageServer)
        {
            m_controller = imageController;
            m_logging = loggingService;
            m_imageServer = imageServer;
        }


        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            new Task(() =>
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    var isFinished = false;
                    while (client.Connected && !isFinished)
                    {
                        // Set buffers for size parts of the message.
                        var headerSizeBytes = new byte[c_sizeBytesCount];
                        var imageSizeBytes = new byte[c_sizeBytesCount];

                        // Read header size.
                        var receivedCount = stream.Read(headerSizeBytes, 0, c_sizeBytesCount);
                        if (receivedCount < c_sizeBytesCount)
                            isFinished = true;
                        else
                        {
                            // Read image size.
                            receivedCount = stream.Read(imageSizeBytes, 0, c_sizeBytesCount);
                            if (receivedCount < c_sizeBytesCount)
                                isFinished = true;
                            else
                            {
                                // Get size values (ints) from byte buffers.
                                var headerSize = ByteArrayToInt(headerSizeBytes);
                                var imageSize = ByteArrayToInt(imageSizeBytes);

                                // Set buffers for the header and body of the message. 
                                var headerBytes = new byte[headerSize];
                                var imageBytes = new byte[imageSize];

                                // Read header.
                                receivedCount = stream.Read(headerBytes, 0, headerSize);

                                if (receivedCount < headerSize)
                                    isFinished = true;
                                else
                                {
                                    // Read body (image).
                                    receivedCount = 0;
                                    while (receivedCount < imageSize) {
                                        receivedCount += stream.Read(imageBytes, receivedCount, imageSize- receivedCount);
                                    }
                                   
                                    // Decode header to string 
                                    var headerString = Encoding.Default.GetString(headerBytes);

                                    // convert the stream of bytes to an image
                                    string handler = m_imageServer.Handlers[0];
                                    Image img = (Bitmap)((new ImageConverter()).ConvertFrom(imageBytes));
                                    img.Save(handler + @"\" + headerString);

                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    clients.Remove(client);
                    client.Close();
                    m_logging.Log(exc.ToString(), MessageTypeEnum.FAIL);
                }
            }).Start();
        }

        public static int ByteArrayToInt(byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            int size = BitConverter.ToInt32(byteArray, 0);
            return size;
        }
    }
}
