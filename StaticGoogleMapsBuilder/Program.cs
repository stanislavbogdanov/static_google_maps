using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Data;
using System.Globalization;
using StaticGoogleMapBuilder;

namespace StaticGoogleMapTest
{
    class Program
    {

        static void Main(string[] args)
        {
            /*
            GMap sgmap = new GMap(700, 450, "AIzaSyA4BeS1xf3tSKmmMDAMg0rxyW972U1Gnm0");
            //sgmap.Scale = 1;
            sgmap.Zoom = 3;
            //sgmap.Mapview = Map.MapView.Terrain;
            sgmap.Center = GMap.Location.FromGeocodingAddress("Australia");
            //sgmap.Language = "en";
            GMap.Location Singapore = GMap.Location.FromGeocodingAddress("Singapore");
            GMap.Location Sydney = GMap.Location.FromGeocodingAddress("Sydney");
            //Map.Location ATH = StaticGMap.Location.FromLatLng(37.936358, 23.944467);

            GMap.MarkersCollection airports = new GMap.MarkersCollection(new GMap.Location[] { Singapore, Sydney });
            airports.Color = GMap.Color.Orange;
            
            //airports.Icon = new Uri(@"http://chart.apis.google.com/chart?chst=d_map_pin_icon&chld=cafe|996600");
            sgmap.Markers.Add(airports);

            GMap.Track route1 = new GMap.Track(new GMap.Location[] { Singapore, Sydney });
            route1.Color = GMap.Color.FromArgb(0xFF, 0xFF, 0, 0);
            //route1.Weight = 8;
            route1.Geodesic = true;
            sgmap.Pathes.Add(route1);

            //Console.WriteLine("---\n{0}\nURL lenght = {1}", sgmap, sgmap.ToString().Length);



            GMap.MapStyle mapst = new GMap.MapStyle(GMap.MapStyleElement.labels);
            mapst.Visibility = GMap.MapStyle.VisibilityEnum.Off;
            sgmap.Styles.Add(mapst);

            mapst = new GMap.MapStyle(GMap.MapStyleElement.geometry_stroke);
            mapst.Visibility = GMap.MapStyle.VisibilityEnum.Off;
            sgmap.Styles.Add(mapst);

            mapst = new GMap.MapStyle(
                GMap.MapStyleFeature.landscape,
                GMap.MapStyleElement.geometry);
            mapst.Saturation = -100.0;
            sgmap.Styles.Add(mapst);

            mapst = new GMap.MapStyle(GMap.MapStyleFeature.water);
            //mapst.Color = Map.Color.Black;
            mapst.Saturation = -100;
            mapst.InvertLightness = true;
            sgmap.Styles.Add(mapst);

            //mapst.Color = Map.Color.FromArgb(0x00ff00);
            //mapst.Weight = 1;
            //mapst.Visibility = Map.MapStyle.VisibilityEnum.On;
            Console.WriteLine(sgmap.ToString());
            Console.WriteLine(sgmap.ToString().Length);

            Bitmap bmp = sgmap.Render();
            bmp.Save("australia.png", System.Drawing.Imaging.ImageFormat.Png);
            */

            DataTable Routes = new DataTable("Routes");
            DataColumn DeptArp = new DataColumn("DeptArp", typeof(string));
            DataColumn DeptLat = new DataColumn("DeptLat", typeof(double));
            DataColumn DeptLng = new DataColumn("DeptLng", typeof(double));
            DataColumn ArvlArp = new DataColumn("ArvlArp", typeof(string));
            DataColumn ArvlLat = new DataColumn("ArvlLat", typeof(double));
            DataColumn ArvlLng = new DataColumn("ArvlLng", typeof(double));
            Routes.Columns.Add(DeptArp);
            Routes.Columns.Add(ArvlArp);
            int cs = ReadSSIM(Routes);
            Console.WriteLine("{0} records in SSIM", cs);
            int uniqroutes = Routes.AsEnumerable().Distinct(DataRowComparer.Default).Count();
            Console.WriteLine("{0} unique routes", uniqroutes);
            /*
            foreach (DataRow dr in Routes.AsEnumerable().Distinct(DataRowComparer.Default))
            {
                for (int g = 0; g < Routes.Columns.Count; g++)
                {
                    Console.Write(dr[g]);
                    Console.Write('\t');
                }
                Console.WriteLine();
            }
            */
            DataTable Airports = new DataTable("Airports");
            DataColumn Iata = new DataColumn("IATA", typeof(string));
            DataColumn Lat = new DataColumn("Lat", typeof(double));
            DataColumn Lng = new DataColumn("Lng", typeof(double));
            Airports.Columns.Add(Iata);
            Airports.Columns.Add(Lat);
            Airports.Columns.Add(Lng);
            int ca = ReadAirportsDB(Airports);
            Console.WriteLine("{0} airports in database", ca);
            Console.WriteLine();

            /*
            List<DataRow> ldr = Routes.AsEnumerable().Distinct(DataRowComparer.Default).ToList();
            ldr.Sort((x, y) => {
                int c = ((string)x["DeptArp"]).CompareTo((string)y["DeptArp"]);
                if (c == 0)
                    return ((string)x["ArvlArp"]).CompareTo((string)y["ArvlArp"]);
                else
                    return c; });
            foreach (var dd in ldr)
            {
                Console.WriteLine("{0} - {1}", dd["DeptArp"], dd["ArvlArp"]);
            }
            */

            
            var DA = (from r in Routes.AsEnumerable().Distinct(DataRowComparer.Default)
                      join d in Airports.AsEnumerable()
                      on r["DeptArp"] equals d["IATA"] into temp
                      from t in temp.DefaultIfEmpty()
                      select new {
                          deptArp = r["DeptArp"],
                          arvlArp = r["ArvlArp"],
                          deptLat = (double?)t["Lat"],
                          deptLng = (double?)t["Lng"]
                      });
            /*
            //var LDD = DD.ToList();
            //LDD.Sort((x, y) => (((string)x.deptArp).CompareTo((string)y.deptArp)));

            var AA = (from r in Routes.AsEnumerable().Distinct(DataRowComparer.Default)
                      join a in Airports.AsEnumerable()
                      on r["ArvlArp"] equals a["IATA"] into temp
                      from t in temp.DefaultIfEmpty()
                      select new
                      {
                          arvlArp = r["ArvlArp"],
                          arvlLat = (double?)t["Lat"],
                          arvlLng = (double?)t["Lng"]
                      });
            */
            var TR = (from da in DA 
                      join aa in Airports.AsEnumerable()
                      on da.arvlArp equals aa["IATA"] into temp
                      from t in temp.DefaultIfEmpty()
                      select new
                      {
                          deptArp = da.deptArp,
                          deptLat = da.deptLat,
                          deptLng = da.deptLng,
                          arvlArp = da.arvlArp,
                          arvlLat = (double?)t["Lat"],
                          arvlLng = (double?)t["Lng"]
                      }
                );
            int m = 0;
            foreach (var tr in TR)
            {
                m++;
                Console.WriteLine("{6,-3} of {7,-3}: {0} {1:F6} {2:F6}\t{3} {4:F6} {5:F6}", 
                    tr.deptArp, tr.deptLat, tr.deptLng,
                    tr.arvlArp, tr.arvlLat, tr.arvlLng,
                    m, uniqroutes);
                if (!CheckAirport((string)tr.deptArp, tr.deptLat) | !CheckAirport((string)tr.arvlArp, tr.arvlLat))
                    continue;
                GMap sgmap = new GMap(700, 450, "AIzaSyA4BeS1xf3tSKmmMDAMg0rxyW972U1Gnm0");
                sgmap.Maptype = GMap.MapType.Terrain;
                GMap.Location dept = GMap.Location.FromLatLng(tr.deptLat.Value, tr.deptLng.Value);
                GMap.Location arvl = GMap.Location.FromLatLng(tr.arvlLat.Value, tr.arvlLng.Value);
                GMap.MarkersCollection arp = new GMap.MarkersCollection(2);
                arp.Add(dept);
                arp.Add(arvl);
                //arp.Icon = new Uri(@"");
                sgmap.Markers.Add(arp);
                GMap.Track route = new GMap.Track();
                route.Add(dept);
                route.Add(arvl);
                route.Geodesic = true;
                route.Color = GMap.Color.FromArgb(0x00ff0000);
                sgmap.Pathes.Add(route);

                string fname = string.Format("{0} - {1}", (string)tr.deptArp, (string)tr.arvlArp);
                if ((string)tr.deptArp == "SVO") fname = (string)tr.arvlArp;
                if ((string)tr.arvlArp == "SVO") fname = (string)tr.deptArp;
                Bitmap bmp = sgmap.Render();
                if (!Directory.Exists(@".\maps"))
                    Directory.CreateDirectory(@".\maps");
                string fullname = @".\maps\" + fname + ".png";
                bmp.Save(fullname, System.Drawing.Imaging.ImageFormat.Png);
            }

            Console.ReadLine();
        }

        private static bool CheckAirport(string code, double? coord)
        {
            if (!coord.HasValue)
                Console.WriteLine("{0} is not found in global airport database.", code);
            return coord.HasValue;
        }

        static int ReadSSIM(DataTable flights)
        {
            string arp1, arp2;
            int recordcounter = 0;
            using (StreamReader currssim = File.OpenText("Current_SSIM"))
            {
                string input = null;
                while ((input = currssim.ReadLine()) != null)
                {
                    if (input[0] != '3') continue; // Processing only for Records Type 3
                    DataRow route = flights.NewRow();
                    arp1 = input.Substring(36, 3);
                    arp2 = input.Substring(54, 3);
                    if (arp1.CompareTo(arp2)>0)
                    {
                        route["DeptArp"] = arp2;
                        route["ArvlArp"] = arp1;
                    }
                    else
                    {
                        route["DeptArp"] = arp1;
                        route["ArvlArp"] = arp2;
                    }
                    flights.Rows.Add(route);
                    recordcounter++;
                }
            }
            //flights.AsEnumerable().Distinct();
            return recordcounter;
        }

        static int ReadAirportsDB(DataTable airports)
        {
            int recordcounter = 0;
            bool header = true;
            using (StreamReader csv = File.OpenText("airports.csv"))
            {
                string input = null;
                while ((input = csv.ReadLine()) != null)
                {
                    if (header) { header = false; continue; }
                    string[] fields = input.Split(new char[] { '\t' });
                    DataRow arploc = airports.NewRow();
                    if (string.IsNullOrWhiteSpace(fields[4])) continue;
                    arploc["IATA"] = fields[4];
                    arploc["Lat"] = double.Parse(fields[6], System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));
                    arploc["Lng"] = double.Parse(fields[7], System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));
                    airports.Rows.Add(arploc);
                    recordcounter++;
                }
            }
            return recordcounter;
        }
    }
}
