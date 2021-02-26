using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Airframes.AirFrameDrivers
{
    public class AirFrameFactory
    {
        public IAirFrameDriver GetAirFrameDriver(string airFrameId) {
            switch (airFrameId)
            {
                case "F-16C":
                    return new F16();
                default:
                    throw new Exception("Unsupported Airframe.");
            }
        }

        public IEnumerable<string> GetAvailableAirFrames()
        {
            return new List<string> { "F-16" };
        }

        private async void Connect()
        {
            try
            {
                var message = "ICP_BTN_4 1\n";

                Int32 port = 7778;
                TcpClient client = new TcpClient("127.0.0.1", port);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                //data = new Byte[256];
                //String responseData = String.Empty;
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }

        private async void CreateErrorDialog(string message)
        {
            // Create a MessageDialog
            var dialog = new MessageDialog(message, "Error");

            // If you want to add custom buttons
            dialog.Commands.Add(new UICommand("Close", delegate (IUICommand command) { }));

            // Show dialog and save result
            var result = await dialog.ShowAsync();
        }
    }
}
