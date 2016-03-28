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
        public struct MarkerSize
        {
            private string _mksize;
            private MarkerSize(string s) { _mksize = s; }

            public static MarkerSize Tiny { get { return new MarkerSize("tiny"); } }
            public static MarkerSize Middle { get { return new MarkerSize("mid"); } }
            public static MarkerSize Small { get { return new MarkerSize("small"); } }

            public bool IsDefined { get { return !string.IsNullOrEmpty(_mksize); } }
            public override string ToString() { return _mksize; }
        }
    }
}

