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
        public struct MapStyle
        {
            private MapStyleFeature _feature;
            private MapStyleElement _element;
            private double _lightness;
            private double _saturation;
            private double _gamma;

            public Color Color { get; set; }
            public Color Hue { get; set; }
            public bool InvertLightness { get; set; }
            public VisibilityEnum Visibility { get; set; }
            public int Weight { get; set; }

            public enum VisibilityEnum { Default, On, Off, Simplified }

            public double Lightness
            {
                get
                {
                    if (_lightness < -100.0) return -100.0;
                    if (_lightness > +100.0) return +100.0;
                    return _lightness;
                }
                set
                {
                    _lightness = value;
                }
            }

            public double Saturation
            {
                get
                {
                    if (_saturation < -100.0) return -100.0;
                    if (_saturation > +100.0) return +100.0;
                    return _saturation;
                }
                set
                {
                    _saturation = value;
                }
            }

            public double Gamma
            {
                get
                {
                    if (_gamma < 0.01) return 0.01;
                    if (_gamma > 10.0) return 10.0;
                    return _gamma;
                }
                set
                {
                    _gamma = value;
                }
            }

            public MapStyle(MapStyleFeature feature, MapStyleElement element)
            {
                _feature = feature;
                _element = element;
                _lightness = 0.0;
                _saturation = 0.0;
                _gamma = 1.0;
                Color = new Color();
                Hue = new Color();
                InvertLightness = false;
                Visibility = VisibilityEnum.Default;
                Weight = -1;
            }

            public MapStyle(MapStyleFeature feature) : this(feature, new MapStyleElement()) { }

            public MapStyle(MapStyleElement element) : this(new MapStyleFeature(), element) { }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                if (!_feature.IsDefined && !_element.IsDefined) return string.Empty;
                if (_feature.IsDefined) sb.AppendFormat("feature:{0}%7C", _feature);
                if (_element.IsDefined) sb.AppendFormat("element:{0}%7C", _element);
                if (Color.IsDefined) sb.AppendFormat("color:{0}%7C", Color.ToString(true));
                if (_lightness != 0.0) sb.AppendFormat("lightness:{0}%7C", _lightness);
                if (_saturation != 0.0) sb.AppendFormat("saturation:{0}%7C", _saturation);
                if (_gamma != 1.0) sb.AppendFormat("gamma:{0}%7C", _gamma);
                if (InvertLightness) sb.AppendFormat("invert_lightness:true%7C");
                if (Visibility != VisibilityEnum.Default)
                {
                    if (Visibility == VisibilityEnum.Off) sb.AppendFormat("visibility:off%7C");
                    if (Visibility == VisibilityEnum.On) sb.AppendFormat("visibility:on%7C");
                    if (Visibility == VisibilityEnum.Simplified) sb.AppendFormat("visibility:simplified%7C");
                }
                if (Weight >= 0) sb.AppendFormat("weight:{0}%7C", Weight);

                string s = sb.ToString();
                return s.Substring(0, s.LastIndexOf("%7C"));   // trim last pipe symbol as %7C
            }
        }

        public struct MapStyleFeature
        {
            private string _msf;
            private MapStyleFeature(string s) { _msf = s; }

            public static MapStyleFeature administrative { get { return new MapStyleFeature("administrative"); } }
            public static MapStyleFeature administrative_country { get { return new MapStyleFeature("administrative.country"); } }
            public static MapStyleFeature administrative_land_parcel { get { return new MapStyleFeature("administrative.land_parcel"); } }
            public static MapStyleFeature administrative_locality { get { return new MapStyleFeature("administrative.locality"); } }
            public static MapStyleFeature administrative_neighborhood { get { return new MapStyleFeature("administrative.neighborhood"); } }
            public static MapStyleFeature administrative_province { get { return new MapStyleFeature("administrative.province"); } }
            public static MapStyleFeature all { get { return new MapStyleFeature("all"); } }
            public static MapStyleFeature landscape { get { return new MapStyleFeature("landscape"); } }
            public static MapStyleFeature landscape_man_made { get { return new MapStyleFeature("landscape.man_made"); } }
            public static MapStyleFeature landscape_natural { get { return new MapStyleFeature("landscape.natural"); } }
            public static MapStyleFeature landscape_natural_landcover { get { return new MapStyleFeature("landscape.natural.landcover"); } }
            public static MapStyleFeature landscape_natural_terrain { get { return new MapStyleFeature("landscape.natural.terrain"); } }
            public static MapStyleFeature poi { get { return new MapStyleFeature("poi"); } }
            public static MapStyleFeature poi_attraction { get { return new MapStyleFeature("poi.attraction"); } }
            public static MapStyleFeature poi_business { get { return new MapStyleFeature("poi.business"); } }
            public static MapStyleFeature poi_government { get { return new MapStyleFeature("poi.government"); } }
            public static MapStyleFeature poi_medical { get { return new MapStyleFeature("poi.medical"); } }
            public static MapStyleFeature poi_park { get { return new MapStyleFeature("poi.park"); } }
            public static MapStyleFeature poi_place_of_worship { get { return new MapStyleFeature("poi.place_of_worship"); } }
            public static MapStyleFeature poi_school { get { return new MapStyleFeature("poi.school"); } }
            public static MapStyleFeature poi_sports_complex { get { return new MapStyleFeature("poi.sports_complex"); } }
            public static MapStyleFeature road { get { return new MapStyleFeature("road"); } }
            public static MapStyleFeature road_arterial { get { return new MapStyleFeature("road.arterial"); } }
            public static MapStyleFeature road_highway { get { return new MapStyleFeature("road.highway"); } }
            public static MapStyleFeature road_highway_controlled_access { get { return new MapStyleFeature("road.highway.controlled_access"); } }
            public static MapStyleFeature road_local { get { return new MapStyleFeature("road.local"); } }
            public static MapStyleFeature transit { get { return new MapStyleFeature("transit"); } }
            public static MapStyleFeature transit_line { get { return new MapStyleFeature("transit.line"); } }
            public static MapStyleFeature transit_station { get { return new MapStyleFeature("transit.station"); } }
            public static MapStyleFeature transit_station_airport { get { return new MapStyleFeature("transit.station.airport"); } }
            public static MapStyleFeature transit_station_bus { get { return new MapStyleFeature("transit.station.bus"); } }
            public static MapStyleFeature transit_station_rail { get { return new MapStyleFeature("transit.station.rail"); } }
            public static MapStyleFeature water { get { return new MapStyleFeature("water"); } }

            public bool IsDefined { get { return !string.IsNullOrEmpty(_msf); } }
            public override string ToString() { return _msf; }
        }

        public struct MapStyleElement
        {
            private string _mse;
            private MapStyleElement(string s) { _mse = s; }

            public static MapStyleElement all { get { return new MapStyleElement("all"); } }
            public static MapStyleElement geometry { get { return new MapStyleElement("geometry"); } }
            public static MapStyleElement geometry_fill { get { return new MapStyleElement("geometry.fill"); } }
            public static MapStyleElement geometry_stroke { get { return new MapStyleElement("geometry.stroke"); } }
            public static MapStyleElement labels { get { return new MapStyleElement("labels"); } }
            public static MapStyleElement labels_icon { get { return new MapStyleElement("labels.icon"); } }
            public static MapStyleElement labels_text { get { return new MapStyleElement("labels.text"); } }
            public static MapStyleElement labels_text_fill { get { return new MapStyleElement("labels.text.fill"); } }
            public static MapStyleElement labels_text_stroke { get { return new MapStyleElement("labels.text.stroke"); } }

            public bool IsDefined { get { return !string.IsNullOrEmpty(_mse); } }
            public override string ToString() { return _mse; }
        }
    }
}

