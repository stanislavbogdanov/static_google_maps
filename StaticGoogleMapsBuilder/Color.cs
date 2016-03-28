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
        public struct Color
        {
            private string name;
            private System.Drawing.Color argbcolor;

            private int ToArgb() { return argbcolor.ToArgb(); }
            private int ToRgba() { return argbcolor.R << 24 | argbcolor.G << 16 | argbcolor.B << 8 | argbcolor.A; }
            private int ToRGB() { return argbcolor.R << 16 | argbcolor.G << 8 | argbcolor.B; }

            public static Color Black { get { return new Color("black"); } }
            public static Color Brown { get { return new Color("brown"); } }
            public static Color Green { get { return new Color("green"); } }
            public static Color Purple { get { return new Color("purple"); } }
            public static Color Yellow { get { return new Color("yellow"); } }
            public static Color Blue { get { return new Color("blue"); } }
            public static Color Gray { get { return new Color("gray"); } }
            public static Color Orange { get { return new Color("orange"); } }
            public static Color Red { get { return new Color("red"); } }
            public static Color White { get { return new Color("white"); } }

            public bool IsDefined { get; }

            private Color(string colorname)
            {
                name = colorname;
                argbcolor = System.Drawing.Color.Empty;
                IsDefined = true;
            }

            private Color(System.Drawing.Color drawcolor)
            {
                name = string.Empty;
                argbcolor = drawcolor;
                IsDefined = true;
            }

            public static Color FromArgb(int argb)
            {
                return new Color(System.Drawing.Color.FromArgb(argb));
            }

            public static Color FromArgb(int red, int green, int blue)
            {
                return new Color(System.Drawing.Color.FromArgb(red, green, blue));
            }

            public static Color FromArgb(int alpha, int red, int green, int blue)
            {
                return new Color(System.Drawing.Color.FromArgb(alpha, red, green, blue));
            }

            public override string ToString()
            {
                return ToString(false);
            }

            public string ToString(bool as24bit)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return "0x" + string.Format("{0:X8}", as24bit ? ToRGB() : ToRgba()).Substring(2);
                else
                    return name;
            }
        }
    }
}

