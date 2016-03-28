using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Net;
using System.IO;

namespace StaticGoogleMapBuilder
{
    public partial class GMap
    {
        public struct Location
        {
            public double Latitude { get; }
            public double Longtitude { get; }
            public string GeocodingAddress { get; }
            public bool IsDefined { get; }

            public static Location Greenwich    // Royal Greenwich Observatory
            { get { return new Location(51.477222, 0.0); } }

            public Location(double lat, double lng)
            {
                lng = lng - 360.0 * (int)(lng / 360.0);
                if (lng > +180.0) lng -= 360.0;
                if (lng < -180.0) lng += 360.0;

                lat = lat - 180.0 * (int)(lat / 180.0);
                if (lat > +90.0) lat = 180.0 - lat;
                if (lat < -90.0) lat = 180.0 + lat;

                if (lat < -90.0 || lat > 90.0)
                    throw new ArgumentOutOfRangeException("Latitude must be in range from -90 to +90.");
                else
                    Latitude = lat;
                if (lng < -180.0 || lng > 180.0)
                    throw new ArgumentOutOfRangeException("Longtitude must be in range from -180 to +180.");
                else
                    Longtitude = lng;
                GeocodingAddress = string.Empty;
                IsDefined = true;
            }

            public Location(string address)
            {
                Latitude = 0.0;
                Longtitude = 0.0;
                GeocodingAddress = address.Trim();
                IsDefined = true;
            }

            public static Location FromLatLng(double lat, double lng)
            {
                return new Location(lat, lng);
            }

            public static Location FromGeocodingAddress(string address)
            {
                return new Location(address);
            }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(GeocodingAddress))
                    return Latitude.ToString("#0.######", CultureInfo.CreateSpecificCulture("en-US")) +
                           "," +
                           Longtitude.ToString("#0.######", CultureInfo.CreateSpecificCulture("en-US"));
                else
                    return Uri.EscapeDataString(GeocodingAddress);
            }
        }
    }
}

