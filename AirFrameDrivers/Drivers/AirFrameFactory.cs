using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Windows.UI.Popups;

namespace AirFrameDrivers.Drivers
{
    public class AirFrameFactory
    {
        public IAirFrameDriver GetAirFrameDriver(string airFrameId)
        {
            switch (airFrameId)
            {
                case "F-16C":
                    return new F16(GetTcpClient());
                default:
                    throw new Exception("Unsupported Airframe.");
            }
        }

        public IEnumerable<string> GetAvailableAirFrames()
        {
            return new List<string> { "F-16C" };
        }

        private TcpClient GetTcpClient()
        {
            Int32 port = 7778;
            TcpClient client = new TcpClient("127.0.0.1", port);
            return client;
        }
    }
}
