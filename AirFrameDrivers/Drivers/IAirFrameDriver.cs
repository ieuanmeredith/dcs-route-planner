using Shared.Models;
using System.Collections.Generic;

namespace AirFrameDrivers.Drivers
{
    public interface IAirFrameDriver
    {
        void SendWaypointsToDcs(IEnumerable<LocationDetails> waypoints);
    }
}
