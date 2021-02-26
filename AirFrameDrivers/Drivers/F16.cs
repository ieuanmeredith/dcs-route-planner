using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace AirFrameDrivers.Drivers
{
    internal enum F16_ICP {
        ICP_BTN_0 = 0,
        ICP_BTN_1 = 1,
        ICP_BTN_2 = 2,
        ICP_BTN_3 = 3,
        ICP_BTN_4 = 4,
        ICP_BTN_5 = 5,
        ICP_BTN_6 = 6,
        ICP_BTN_7 = 7,
        ICP_BTN_8 = 8,
        ICP_BTN_9 = 9,
        ICP_DATA_RTN_SEQ_SW,
        ICP_DATA_UP_DN_SW,
        ICP_ENTR_BTN
    }
    internal sealed class F16 : IAirFrameDriver
    {
        private TcpClient _client;

        public F16(TcpClient client)
        {
            _client = client;
        }

        public void SendWaypointsToDcs(IEnumerable<LocationDetails> waypoints)
        {
            var wpArr = waypoints.ToArray();
            var stream = _client.GetStream();

            SendCommand(F16_ICP.ICP_DATA_RTN_SEQ_SW, stream);
            SendCommand(F16_ICP.ICP_BTN_4, stream);

            for (var i = 0; i < waypoints.Count(); i++)
            {
                // Enter Waypoint Number
                SendCommand((F16_ICP)(i + 1), stream);
                SendCommand(F16_ICP.ICP_ENTR_BTN, stream);

                // Down
                SendCommand(F16_ICP.ICP_DATA_UP_DN_SW, stream);

                // Enter Lat
                SendCommand(GetCardinalButton(wpArr[i].LatDir), stream);
                var latArr = wpArr[i].GetLatDmsCharArray();
                for (var c = 0; c < 7; c++)
                {
                    SendCommand((F16_ICP)Char.GetNumericValue(latArr[c]), stream);
                }
                SendCommand(F16_ICP.ICP_ENTR_BTN, stream);

                //Down
                SendCommand(F16_ICP.ICP_DATA_UP_DN_SW, stream);

                // Enter Lon
                SendCommand(GetCardinalButton(wpArr[i].LonDir), stream);
                var lonArr = wpArr[i].GetLonDmsCharArray();
                for (var c = 0; c < 8; c++)
                {
                    SendCommand((F16_ICP)Char.GetNumericValue(lonArr[c]), stream);
                }
                SendCommand(F16_ICP.ICP_ENTR_BTN, stream);

                //Down
                SendCommand(F16_ICP.ICP_DATA_UP_DN_SW, stream);

                // Enter Alt
                foreach (var c in wpArr[i].GetAltCharArray())
                {
                    SendCommand((F16_ICP)Char.GetNumericValue(c), stream);
                }

                // Down, Down
                SendCommand(F16_ICP.ICP_DATA_UP_DN_SW, stream);
                SendCommand(F16_ICP.ICP_DATA_UP_DN_SW, stream);
            }

            SendCommand(F16_ICP.ICP_DATA_RTN_SEQ_SW, stream);

            stream.Close();
            _client.Close();
        }

        public void SendCommand(F16_ICP button, NetworkStream stream)
        {
            switch (button)
            {
                case F16_ICP.ICP_DATA_RTN_SEQ_SW:
                case F16_ICP.ICP_DATA_UP_DN_SW:
                    StreamWrite(button, 0, stream);

                    Thread.Sleep(100);

                    StreamWrite(button, 1, stream);
                    break;
                default:
                    StreamWrite(button, 1, stream);

                    Thread.Sleep(100);

                    StreamWrite(button, 0, stream);
                    break;
            }

            Thread.Sleep(100);
        }

        private F16_ICP GetCardinalButton(string dir)
        {
            switch (dir)
            {
                case "N":
                    return F16_ICP.ICP_BTN_2;
                case "E":
                    return F16_ICP.ICP_BTN_6;
                case "S":
                    return F16_ICP.ICP_BTN_8;
                case "W":
                    return F16_ICP.ICP_BTN_4;
                default:
                    return F16_ICP.ICP_DATA_RTN_SEQ_SW;
            }
        }

        private void StreamWrite(F16_ICP button, int value, NetworkStream stream)
        {
            var message = $"{button.ToString()} {value}\n";
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
