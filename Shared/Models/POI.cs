using System;
using System.Collections.Generic;

namespace Shared.Models
{
    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public Point Point { get; set; }
        public double Radius { get; set; }
    }

    public class LocationDetails
    {
        public double Altitude { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Mgrs { get; set; }
        public string DmsString { get { return this.ToDmsString(); } }

        public string LatDir { get { return this.Lat >= 0 ? "N" : "S"; } }
        public string LonDir { get { return this.Lat >= 0 ? "E" : "W"; } }

        public char[] GetLatDmsCharArray() {
            double lat = this.Lat;

            lat = Math.Abs(lat);
            double latMinPart = ((lat - Math.Truncate(lat) / 1) * 60);
            double latSecPart = ((latMinPart - Math.Truncate(latMinPart) / 1) * 60);
            double latMsPart = ((latSecPart - Math.Truncate(latSecPart) / 1) * 100);

            string latString = $"{Math.Truncate(lat).ToString("00")}{Math.Truncate(latMinPart).ToString("00")}{Math.Truncate(latSecPart).ToString("00")}{Math.Truncate(latMsPart).ToString("0")}";
            return latString.ToCharArray();
        }

        public char[] GetLonDmsCharArray()
        {
            double lon = this.Lon;

            lon = Math.Abs(lon);
            double lonMinPart = ((lon - Math.Truncate(lon) / 1) * 60);
            double lonSecPart = ((lonMinPart - Math.Truncate(lonMinPart) / 1) * 60);
            double lonMsPart = ((lonSecPart - Math.Truncate(lonSecPart) / 1) * 100);

            var longString = $"{Math.Truncate(lon).ToString("000")}{Math.Truncate(lonMinPart).ToString("00")}{Math.Truncate(lonSecPart).ToString("00")}{Math.Truncate(lonMsPart).ToString("0")}";
            return longString.ToCharArray();
        }

        public char[] GetAltCharArray()
        {
            return Convert.ToInt32(this.Altitude).ToString().ToCharArray();
        }

        private string ToDmsString()
        {
            double lat = this.Lat;
            double lon = this.Lon;

            string latDir = (lat >= 0 ? "N" : "S");
            lat = Math.Abs(lat);
            double latMinPart = ((lat - Math.Truncate(lat) / 1) * 60);
            double latSecPart = ((latMinPart - Math.Truncate(latMinPart) / 1) * 60);
            double latMsPart = ((latSecPart - Math.Truncate(latSecPart) / 1) * 100);

            string lonDir = (lon >= 0 ? "E" : "W");
            lon = Math.Abs(lon);
            double lonMinPart = ((lon - Math.Truncate(lon) / 1) * 60);
            double lonSecPart = ((lonMinPart - Math.Truncate(lonMinPart) / 1) * 60);
            double lonMsPart = ((lonSecPart - Math.Truncate(lonSecPart) / 1) * 100);


            var latString = Math.Truncate(lat) + "°" + Math.Truncate(latMinPart) + "'" + Math.Truncate(latSecPart) + "." + Math.Truncate(latMsPart) + "\"" + latDir;
            var longString = Math.Truncate(lon) + "°" + Math.Truncate(lonMinPart) + "'" + Math.Truncate(lonSecPart) + "." + Math.Truncate(lonMsPart) + "\"" + lonDir;

            return latString + " " + longString;
        }
    }

    public class POI
    {
        public List<Location> Location { get; set; }
        public LocationDetails LocationDetails { get; set; }
    }
}
