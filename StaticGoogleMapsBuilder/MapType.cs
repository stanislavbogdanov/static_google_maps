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
        public struct MapType
        {
            private string _mtype;
            private MapType(string s) { _mtype = s; }

            public static MapType Roadmap { get { return new MapType("roadmap"); } }
            public static MapType Satellite { get { return new MapType("satellite"); } }
            public static MapType Hybrid { get { return new MapType("hybrid"); } }
            public static MapType Terrain { get { return new MapType("terrain"); } }

            public bool IsDefined { get { return !string.IsNullOrEmpty(_mtype); } }
            public override string ToString() { return _mtype; }
        }
    }
}

