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
        public class Track : LocationList
        {
            public int Weight { get; set; }
            public Color Color { get; set; }
            public Color FillColor { get; set; }
            public bool Geodesic { get; set; }
            public bool EncodedPolyline { get; set; }

            public Track() : base() { }
            public Track(IEnumerable<Location> trackpoints) : base(trackpoints) { }
            public Track(int capacity) : base(capacity) { }

            /*
            public static string ToBinary(int x, int splitAt = 0, char separator = ' ')
            {
                int sepnum = (splitAt > 0) ? (8 * sizeof(int) / splitAt - ((8 * sizeof(int) % splitAt == 0) ? 1 : 0)) : 0;
                int arrc = 8 * sizeof(int) + sepnum;
                int sepc = (splitAt > 0) ? splitAt : (8 * sizeof(int) + 1);
                char[] binary = new char[arrc];
                while (--arrc >= 0)
                {
                    binary[arrc] = ((x & 1) == 1) ? '1' : '0';
                    x >>= 1;
                    if (--sepc == 0)
                    {
                        sepc = splitAt;
                        if (arrc > 0) binary[--arrc] = separator;
                    }
                }
                return new String(binary);
            }
            */

            private string EncodeCoordinate(double n)
            {
                int num = (int)Math.Round(n * 1e5);
                num <<= 1;
                if (n < 0) num = ~num;

                StringBuilder sb = new StringBuilder();
                while (num >= 0x20)
                {
                    sb.Append((char)(((num & 0x1F) | 0x20) + 63));
                    num >>= 5;
                }
                sb.Append((char)(num + 63));
                return sb.ToString();
            }

            public string Encode()
            {
                StringBuilder encodedstr = new StringBuilder();
                double lastLat = 0.0;
                double lastLng = 0.0;
                foreach (Location loc in this)
                {
                    encodedstr.Append(EncodeCoordinate(loc.Latitude - lastLat));
                    encodedstr.Append(EncodeCoordinate(loc.Longtitude - lastLng));
                    lastLat = loc.Latitude;
                    lastLng = loc.Longtitude;
                }
                return encodedstr.ToString().Replace(@"\", @"\\");
            }

            private static IEnumerable<double> Coordinates(string encstr)
            {
                int lastindex;
                int index = 0;
                while (index < encstr.Length)
                {
                    lastindex = index;
                    while (index < encstr.Length && encstr[index] >= (0x20 + 63)) index++;
                    index++;
                    yield return DecodeCoordinate(encstr.Substring(lastindex, index - lastindex));
                }
            }

            private static double DecodeCoordinate(string encstr)
            {
                int bitrow = 0;
                for (int i = encstr.Length - 1; i >= 0; i--)
                {
                    bitrow <<= 5;
                    bitrow |= (encstr[i] - 63) & 0x1F;
                }
                bitrow = ((bitrow & 1) == 1) ? ~(bitrow >> 1) : (bitrow >> 1);
                return bitrow / 1e5;
            }

            public static IEnumerable<Location> Decode(string encstr)
            {
                Track path = new Track();
                double lat = 0.0;
                double lng = 0.0;
                bool setlat = false;
                foreach (double num in Coordinates(encstr))
                {
                    if (setlat)
                    {
                        lng += num;
                        setlat = false;
                        yield return new Location(lat, lng);
                    }
                    else
                    {
                        lat += num;
                        setlat = true;
                    }
                }
            }

            public override string ToString()
            {
                if (Count == 0) return string.Empty;
                StringBuilder sb = new StringBuilder(MAX_URL_LEN);
                if (Weight > 0) sb.AppendFormat("weight:{0}%7C", Weight);
                if (Color.IsDefined) sb.AppendFormat("color:{0}%7C", Color);
                if (FillColor.IsDefined) sb.AppendFormat("fillcolor:{0}%7C", Color);
                if (Geodesic) sb.AppendFormat("geodesic:{0}%7C", Geodesic.ToString().ToLower());
                if (EncodedPolyline)
                    sb.AppendFormat("enc:{0}", Encode());
                else
                    sb.Append(base.ToString());
                return sb.ToString();
            }
        }
    }
}

