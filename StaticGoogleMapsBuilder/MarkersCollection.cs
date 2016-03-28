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
        public class MarkersCollection : LocationList
        {
            public MarkerSize Size { get; set; }
            public Color Color { get; set; }
            public char Label { get; set; }
            public Uri Icon { get; set; }

            private MarkersCollection() : base() { }
            public MarkersCollection(IEnumerable<Location> points) : base(points) { }
            public MarkersCollection(int capacity) : base(capacity) { }

            public override string ToString()
            {
                if (Count == 0) return string.Empty;
                StringBuilder sb = new StringBuilder(MAX_URL_LEN);
                if (Icon != null)
                {
                    UriBuilder iconub = new UriBuilder(Icon);
                    iconub.Query = Uri.EscapeDataString(iconub.Query.Replace("?", ""));
                    iconub.Fragment = Uri.EscapeDataString(iconub.Fragment.Replace("#", ""));
                    sb.AppendFormat("icon:{0}%7C", iconub.Uri.ToString());
                }
                else
                {
                    if (Size.IsDefined) sb.AppendFormat("size:{0}%7C", Size);
                    if (Color.IsDefined) sb.AppendFormat("color:{0}%7C", Color.ToString(true));
                    if (Label != default(char)) sb.AppendFormat("label:{0}%7C", Label.ToString().ToUpper());
                }
                return sb.Append(base.ToString()).ToString();
            }
        }
    }
}

