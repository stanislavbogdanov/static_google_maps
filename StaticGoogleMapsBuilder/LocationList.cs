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
        public class LocationList : List<Location>
        {
            public LocationList() : base() { }
            public LocationList(IEnumerable<Location> collection) : base(collection) { }
            public LocationList(int capacity) : base(capacity) { }

            public override string ToString()
            {
                return string.Join("%7C", this);
            }
        }
    }
}

