using MapleLib.WzLib;
using MapleLib.WzLib.Serializer;
using MapleLib.WzLib.WzProperties;

namespace HarepackerCLI.Converters
{
    internal class MapConverter
    {
        public static void Convert(string[] args)
        {
            if (args.Length < 7)
            {
                Console.WriteLine("Usage: HarepackerCLI.exe convert map <data-directory> <game-version> <map-id> <reactor-objects.txt> <output-img>");
                Console.WriteLine("\nreactor-objects.txt example:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(@"
2208004=tower/toyTowerInside/elevator
2221000=darkmountain/artificiality/fvMob0
2221001=darkmountain/artificiality/fvMob1
2221002=darkmountain/nature/fvMob2
2221003=folkvillige/artificiality/fvquest0
2221004=folkvillige/artificiality/fvquest1
2222000=darkmountain/nature/fvquest2
                    ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(1);
            }
            else
            {
                WzFile inputWz = new WzFile(Path.Join(args[2], "Data.wz"), args[3], WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(inputWz);
                int mapID = int.Parse(args[4]);
                string reactorObjectsFile = args[5];
                string outputImg = args[6];
                Convert(inputWz, mapID, reactorObjectsFile, outputImg);
            }
        }
        private static void Convert(WzFile inputWz, int mapID, string reactorObjectsFile, string outputImg)
        {
            var reactorObjects = File.ReadAllLines(reactorObjectsFile).Select(i => i.Split("=")).ToDictionary(i => i[0], j => j[1]);
            try
            {
                var mapImg = (WzImage)inputWz["Map"]["Map" + mapID.ToString()[0]][mapID + ".img"];

                foreach (var mapProp in mapImg.WzProperties)
                {
                    if (mapProp.Name == "reactor")
                    {
                        foreach (var reactorProp in mapProp.WzProperties)
                        {
                            var id = reactorProp["id"].GetString();
                            var objPaths = reactorObjects[id].Split("/");

                            int x = reactorProp["x"].GetInt();
                            int y = reactorProp["y"].GetInt();
                            var nearestFH = FindNearestFoothold(mapImg, x, y);
                            int layer;
                            if (nearestFH == null)
                            {
                                Console.WriteLine($"Couldn't find nearest fh for reactor {id}, defaulting to layer 3");
                                layer = 3;
                            }
                            else
                            {
                                layer = int.Parse(nearestFH.Parent.Parent.Name);
                            }
                            var layerProp = mapImg[layer.ToString()]["obj"];
                            //int zM = FindHighestPlatform(layerProp);
                            var pieceIdx = layerProp.WzProperties.Count;
                            var reactorObjProp = new WzSubProperty(pieceIdx.ToString());
                            reactorObjProp.WzProperties.Add(new WzStringProperty("oS", "Reactor"));
                            for (int i = 0; i < objPaths.Length; i++)
                            {
                                reactorObjProp.WzProperties.Add(new WzStringProperty("l" + i, objPaths[i]));
                            }
                            reactorObjProp.WzProperties.Add(new WzIntProperty("x", x));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("y", y));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("z", 200));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("zM", 0));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("f", reactorProp["f"].GetInt()));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("reactorTime", reactorProp["reactorTime"].GetInt()));
                            reactorObjProp.WzProperties.Add(new WzIntProperty("reactor", 1));

                            layerProp.WzProperties.Add(reactorObjProp);

                            reactorProp.WzProperties.RemoveAll(i => true);
                            reactorProp.WzProperties.Add(new WzIntProperty("pageIdx", layer));
                            reactorProp.WzProperties.Add(new WzIntProperty("pieceIdx", pieceIdx));
                        }
                    }
                }

                var serializer = new WzImgSerializer();
                mapImg.Changed = true;
                serializer.SerializeImage(mapImg, outputImg);
            }
            catch (ThreadAbortException)
            {
                Environment.Exit(1);
            }
        }

        private static WzImageProperty FindNearestFoothold(WzImage mapImg, int x, int y)
        {
            (WzImageProperty footholdProp, float distSq) nearest = (null, 0);
            foreach (var layerProp in mapImg["foothold"].WzProperties)
            {
                foreach (var zmProp in layerProp.WzProperties)
                {
                    foreach (var footholdProp in zmProp.WzProperties)
                    {
                        int x1 = footholdProp["x1"].GetInt();
                        int x2 = footholdProp["x2"].GetInt();
                        int y1 = footholdProp["y1"].GetInt();
                        int y2 = footholdProp["y2"].GetInt();

                        if (IsPointNearLineSegment(x1, y1, x2, y2, x, y, out float distSq, 10) && (distSq < nearest.distSq || nearest.footholdProp == null))
                        {
                            nearest = (footholdProp, distSq);
                        }
                    }
                }
            }
            return nearest.footholdProp;
        }

        private static int FindHighestPlatform(WzImageProperty layerProp)
        {
            int highestPlatform = 0;
            foreach (var layerObjProp in layerProp.WzProperties)
            {
                int platform = layerObjProp["zM"].GetInt();
                if (platform > highestPlatform) highestPlatform = platform;
            }
            return highestPlatform;
        }

        /// <summary>
        /// Checks if a point (px, py) is within 'margin' distance of a line segment (x1, y1) - (x2, y2).
        /// </summary>
        public static bool IsPointNearLineSegment(
            float x1, float y1, float x2, float y2,
            float px, float py,
            out float distSq,
            float margin = 5f
        )
        {
            // Compute squared length of the line segment
            float dx = x2 - x1;
            float dy = y2 - y1;
            float lenSq = dx * dx + dy * dy;

            if (lenSq == 0f)
            {
                // Line segment is just a point
                distSq = (px - x1) * (px - x1) + (py - y1) * (py - y1);
                return distSq <= margin * margin;
            }

            // Compute projection factor (t) of point onto the line (normalized 0..1)
            float t = ((px - x1) * dx + (py - y1) * dy) / lenSq;
            t = Math.Max(0, Math.Min(1, t)); // clamp to segment

            // Find the closest point on the segment
            float closestX = x1 + t * dx;
            float closestY = y1 + t * dy;

            // Compute distance from point to the closest point
            float distX = px - closestX;
            float distY = py - closestY;
            distSq = distX * distX + distY * distY;

            return distSq <= margin * margin;
        }
    }
}
