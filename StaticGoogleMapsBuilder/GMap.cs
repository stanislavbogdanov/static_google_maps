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
    /// <summary>
    /// The class GMap uses the Google Static Maps API and returns an image in PNG8 as non-destructive compression 
    /// format in response to an HTTP request via a URL. For each request, you can specify the location of the map, 
    /// the size of the image, the zoom level, the type of map, and the placement of optional markers at locations 
    /// on the map. You can additionally label your markers using alphanumeric characters.
    /// For more information, see https://developers.google.com/maps/documentation/static-maps/intro
    /// </summary>
    public partial class GMap
    {
        #region Key and Signature Parameters
        /// <summary>
        /// GoogleAPIKey (always required) allows you to monitor your application's API usage in the Google 
        /// Developers Console; enables per-key instead of per-IP-address quota limits; and ensures that Google 
        /// can contact you about your application if necessary.  
        /// </summary>
        public string GoogleAPIKey { get; }             // Always required 

        /// <summary>
        /// Signature (recommended) is a digital signature used to verify that any site generating requests using 
        /// your API key is authorized to do so. 
        /// </summary>
        public string Signature { get; set; }           // Recommended
        #endregion

        #region Location Parameters
        /// <summary>
        /// Center (required if markers not present) defines the center of the map, equidistant from all edges 
        /// of the map.
        /// </summary>
        public Location Center { get; set; }            // Required if Markers not present

        /// <summary>
        /// Zoom (required if markers not present) defines the zoom level of the map (in range from 0 to 22), 
        /// which determines the magnification level of the map. This parameter takes a numerical value corresponding 
        /// to the zoom level of the region desired. 
        /// </summary>
        public int Zoom { get; set; }                   // Required if Markers not present
        #endregion

        #region Map Parameters
        /// <summary>
        /// Width (always required) defines the horizontal dimension of the map image. This parameter is affected 
        /// by the scale parameter; the final output size is the product of the size and scale values.
        /// </summary>
        public int Width { get; }                       // Always required

        /// <summary>
        /// Height (always required) defines the vertical dimension of the map image. This parameter is affected 
        /// by the scale parameter; the final output size is the product of the size and scale values.
        /// </summary>
        public int Height { get; }                      // Always required

        /// <summary>
        /// Scale (optional) affects the number of pixels that are returned. 
        /// </summary>
        public int Scale { get; set; }                  // Optional
        /* public MapFormat ImageFormat { get; set; } */

        /// <summary>
        /// Maptype (optional) defines the type of map to construct. There are several possible maptype values, including 
        /// roadmap, satellite, hybrid, and terrain. 
        /// </summary>
        public MapType Maptype { get; set; }            // Optional

        /// <summary>
        /// Language (optional) defines the language to use for display of labels on map tiles. 
        /// </summary>
        public string Language { get; set; }            // Optional

        /// <summary>
        /// Region (optional) defines the appropriate borders to display, based on geo-political sensitivities. 
        /// </summary>
        public string Region { get; set; }              // Optional
        #endregion

        #region Feature Parameters
        /// <summary>
        /// Markers (optional) define the list of single style markers sets. Note that if you supply markers for a map, 
        /// you do not need to specify the (normally required) center and zoom parameters.  
        /// </summary>
        public List<MarkersCollection> Markers;         // Optional

        /// <summary>
        /// Pathes (optional) define the list of single style tracks. Note that if you supply a path for a map, 
        /// you do not need to specify the (normally required) center and zoom parameters. 
        /// </summary>
        public List<Track> Pathes;                      // Optional

        /// <summary>
        /// Viewports (optional) specifies the list of locations that should remain visible on the map, though no markers 
        /// or other indicators will be displayed. Corresponds to the 'visible' parameter of the Google Static Maps API.
        /// </summary>
        public LocationList Viewports;                  // Optional

        /// <summary>
        /// Styles (optional) defines the list of custom styles to alter the presentation of a specific feature 
        /// (road, park, etc.) of the map.
        /// </summary>
        public List<MapStyle> Styles;                   // Optional

        /// <summary>
        /// Additional parameter reserved for future use
        /// </summary>
        public string CustomParameter { get; set; }     // Optional
        #endregion

        private WebClient wc = new WebClient();

        private GMap() { }

        /// <summary>
        /// Initializes a new instance of the GMap class with the specified size of image and existing API key.
        /// </summary>
        public GMap(int width, int height, string google_api_key)
        {
            Width = width;
            Height = height;
            GoogleAPIKey = google_api_key;

            Zoom = -1; Scale = -1; // Negative values mean "undefined"

            Markers = new List<MarkersCollection>();
            Pathes = new List<Track>();
            Viewports = new LocationList();
            Styles = new List<MapStyle>();

            //wc = new WebClient();
            wc.Proxy = WebRequest.GetSystemWebProxy();
            wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
        }

        /// <summary>
        /// Initializes a new instance of the GMap class with the specified size of image, center of the map, 
        /// zoom value and existing API key.
        /// </summary>
        public GMap(int width, int height, Location center, int zoom, string google_api_key) : this(width, height, google_api_key)
        {
            Center = center;
            Zoom = zoom;
        }

        /// <summary>
        /// Sends the request to the Google and returns the Bitmap image. 
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Net.WebException"></exception>
        public Bitmap Render()
        {
            byte[] buffer = wc.DownloadData(this.ToUri());   // Send request
            return GetBitmap(buffer);
        }

        /// <summary>
        /// Sends the request to the Google in an asynchronous manner and returns the Bitmap image. 
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Net.WebException"></exception>
        public async Task<Bitmap> RenderAsync()
        {
            return await Task.Run(async () =>
            {
                byte[] buffer = await wc.DownloadDataTaskAsync(this.ToUri());   // Send request
                return GetBitmap(buffer);
            });
        }

        private Bitmap GetBitmap(byte[] buffer)
        {
            Bitmap bmp = null;
            using (MemoryStream mems = new MemoryStream(buffer))
            {
                bmp = new Bitmap(mems);
            }
            return bmp;
        }

        /// <summary>
        /// Gets any query information included in the builded URI.
        /// </summary>
        public string Query
        {
            get
            {
                StringBuilder sb = new StringBuilder("?", MAX_URL_LEN);
                sb.AppendFormat("size={0}x{1}", Width, Height);

                if (Center.IsDefined) sb.AppendFormat("&center={0}", Center);
                if (Zoom >= 0) sb.AppendFormat("&zoom={0}", Zoom);
                if (Scale >= 0) sb.AppendFormat("&scale={0}", Scale);
                if (Maptype.IsDefined) sb.AppendFormat("&maptype={0}", Maptype);
                if (!string.IsNullOrWhiteSpace(Language)) sb.AppendFormat("&language={0}", Language.Trim());
                if (!string.IsNullOrWhiteSpace(Region)) sb.AppendFormat("&region={0}", Region.Trim());
                foreach (MarkersCollection ms in Markers)
                    if (!string.IsNullOrWhiteSpace(ms.ToString()))
                        sb.AppendFormat("&markers={0}", ms);
                foreach (Track p in Pathes)
                    if (!string.IsNullOrWhiteSpace(p.ToString()))
                        sb.AppendFormat("&path={0}", p);
                foreach (Location vp in Viewports)
                    if (!string.IsNullOrWhiteSpace(vp.ToString()))
                        sb.AppendFormat("&visible={0}", vp);
                foreach (MapStyle style in Styles)
                    if (!string.IsNullOrWhiteSpace(style.ToString()))
                        sb.AppendFormat("&style={0}", style);
                if (!string.IsNullOrWhiteSpace(CustomParameter)) sb.Append("&" + CustomParameter);
                sb.AppendFormat("&key={0}", GoogleAPIKey);
                if (!string.IsNullOrWhiteSpace(Signature)) sb.AppendFormat("&signature={0}", Signature);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets a string representation for the buided URI.
        /// </summary>
        public override string ToString()
        {
            return BASEURL + Query;
        }

        /// <summary>
        /// Gets the builded URI for the request to the Google.
        /// </summary>
        public Uri ToUri()
        {
            return new Uri(ToString());
        }

        /*
        public struct MapFormat
        {
            private string mformat;

            public static MapFormat PNG { get { return new MapFormat { mformat = "png" }; } }
            public static MapFormat PNG8 { get { return new MapFormat { mformat = "png8" }; } }
            public static MapFormat PNG32 { get { return new MapFormat { mformat = "png32" }; } }
            public static MapFormat GIF { get { return new MapFormat { mformat = "gif" }; } }
            public static MapFormat JPEG { get { return new MapFormat { mformat = "jpg" }; } }
            public static MapFormat NonprogressiveJPEG { get { return new MapFormat { mformat = "jpg-baseline" }; } }

            public override string ToString() { return mformat; }
        }
        */

        private const int MAX_URL_LEN = 2000;
        private const string BASEURL = @"https://maps.googleapis.com/maps/api/staticmap";
    }
}
