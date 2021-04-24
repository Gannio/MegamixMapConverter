using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace MegamixMapConverter
{
    class Program
    {

        static List<List<string>> songList;
        static Bitmap borderTiles;
        static Bitmap megamanImage;
        static Bitmap megamanImage910;
        static bool indivMode = false;
        static bool estimateColors = true;
        static bool doManualNESMode = indivMode;
        static bool manualNESMode = false;
        static bool overwriteExisting = false;
        static bool noNameFixes = false;
        static bool singleThreaded = false;
        static bool skipMega = false;
        static bool skipBorders = false;
        static int tsaValue = 16;
        static void Main(string[] args)
        {
            bool directoryFault = false;
            if (!Directory.Exists("maps"))
            {
                directoryFault = true;
                Directory.CreateDirectory("maps");
                Console.WriteLine("Please insert your image files in the maps folder! Compatible maps are at vgmaps.com by Revned.");
            }
            if (!Directory.Exists("tilesets"))
            {
                directoryFault = true;
                Directory.CreateDirectory("tilesets");
                Console.WriteLine("Please insert your tilesets in the tilesets folder! Megamix tilesets will work for most instances.");
            }
            if (!Directory.Exists("rooms"))
            {
                Directory.CreateDirectory("rooms");
            }
            if (!File.Exists("megaman.png"))
            {
                Console.WriteLine("Please add 'megaman.png' (NES Mega Man).");
                directoryFault = true;
            }
            if (!File.Exists("megaman2.png"))
            {
                Console.WriteLine("Please add 'megaman2.png' (Wii Mega Man).");
                directoryFault = true;
            }
            if (!File.Exists("borderTiles.png"))
            {
                Console.WriteLine("Please add 'borderTiles.png' from a release build.");
                directoryFault = true;
            }
            if (!File.Exists("songlist.txt"))
            {
                Console.WriteLine("Please add 'songlist.txt' from a release build.");
                directoryFault = true;
            }
            if (!File.Exists("lvlCopyThisRoom.room.gmx"))
            {
                Console.WriteLine("Please add 'lvlCopyThisRoom.room.gmx' from the Megamix Engine.");
                directoryFault = true;
            }
            if (directoryFault)
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                return;
            }
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-i":
                        case "--individualMode":
                            indivMode = true;
                            break;
                        case "-e":
                        case "--exactTileCheck":
                            estimateColors = false;
                            break;
                        case "-o":
                        case "--overwriteExisting":
                            overwriteExisting = true;
                            break;
                        case "-n":
                        case "--namesExact":
                            noNameFixes = true;
                            break;
                        case "-s":
                        case "--singleThread":
                            singleThreaded = true;
                            break;
                        case "-m":
                        case "--skipMega":
                            skipMega = true;
                            break;
                        case "-b":
                        case "--skipBorders":
                            skipBorders = true;
                            break;
                        case "-h":
                        case "--halfTiles":
                            Console.WriteLine("Note: Half-tile currently requires a modified auto-tiler for it to not be laggy!");
                            tsaValue = 8;//skipBorders = true;
                        break;
                    }
                }

            }
            Console.WriteLine("Hello World!");
            Console.WriteLine("Choose Map from list below:");
            List<string> files = new List<string>();
            if (indivMode)
            {
                foreach (string file in Directory.GetFiles("maps"))
                {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(file));
                }
                string fileName = Console.ReadLine();
                files.Add(fileName);
            }
            else
            {
                foreach (string file in Directory.GetFiles("maps"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    files.Add(fileName);
                    //files.Add(fileName + "Animated1");
                    Console.WriteLine(fileName);
                }
            }
            if (doManualNESMode)
            {
                Console.WriteLine("NES mode or Wii Mode? y/n");

                if (Console.ReadLine() == "y")
                {
                    manualNESMode = true;
                }
                else
                {
                    manualNESMode = false;
                }
            }
            borderTiles = Bitmap.FromFile("borderTiles.png") as Bitmap;
            if (borderTiles.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed || borderTiles.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || borderTiles.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {//If map is in index format, change to non-indexed.
             //using (Bitmap oldBmp = new Bitmap("myimage.jpg"))
                using (Bitmap newBmp = new Bitmap(borderTiles))
                using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    borderTiles = targetBmp.Clone() as Bitmap;// targetBmp is now in the desired format.
                }
            }

            megamanImage = Bitmap.FromFile("megaman.png") as Bitmap;
            if (megamanImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed || megamanImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || megamanImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {//If map is in index format, change to non-indexed.
             //using (Bitmap oldBmp = new Bitmap("myimage.jpg"))
                using (Bitmap newBmp = new Bitmap(megamanImage))
                using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    megamanImage = targetBmp.Clone() as Bitmap;// targetBmp is now in the desired format.
                }
            }

            megamanImage910 = Bitmap.FromFile("megaman2.png") as Bitmap;
            if (megamanImage910.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed || megamanImage910.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || megamanImage910.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {//If map is in index format, change to non-indexed.
             //using (Bitmap oldBmp = new Bitmap("myimage.jpg"))
                using (Bitmap newBmp = new Bitmap(megamanImage910))
                using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    megamanImage910 = targetBmp.Clone() as Bitmap;// targetBmp is now in the desired format.
                }
            }


            songList = new List<List<string>>();
            string songText = File.ReadAllText("songlist.txt");
            List<string> nsfs = new List<string>(songText.Split("\r\n\r\n"));
            foreach (string nsf in nsfs)
            {
                List<string> trackList = new List<string>(nsf.Split("\r\n"));
                songList.Add(trackList);
            }


            PerformAllConversions(files);


            Console.ReadLine();
        }

        static async void PerformAllConversions(List<string> files)
        {
            Task[] myTasks = new Task[files.Count];
            int i = 0;
            //foreach (string mapName in files)
            //{
            Task<string>[] outputs = new Task<string>[files.Count];
            int parallelMax = -1;
            if (singleThreaded)
            {
                parallelMax = 1;
            }
            Parallel.For(0, files.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelMax }, i =>
            {
                string mapName = files[i];
                Console.WriteLine(i);
                myTasks[i] = Task.Run(() => { outputs[i] = PerformConversion(i, mapName); });
                //myTasks[i].Start();
                //i++;
            });


            //}
            await Task.WhenAll(myTasks);

            Console.WriteLine("Fully done! Report Card:");
            /*foreach (Task<string> output in outputs)
            {
                Console.WriteLine(output.Result);
                //Console.WriteLine((string)output);
            }*/
            for (int a = 0; a < outputs.Length; a++)
            {
                Console.WriteLine(outputs[a].Result);
            }
            return;
        }

        static async Task<string> PerformConversion(int index, string mapName)
        {
//            await Task.Run(() => { });
            Random rand = new Random(new System.DateTime().Millisecond + index);
            List<uint> rand_CalledValues = new List<uint>();//Used to ensure same random value is not called twice.
            
            //string mapName = Console.ReadLine();
            /*Console.WriteLine("Choose Tileset from list below:");
            foreach (string file in Directory.GetFiles("tilesets"))
            {
                Console.WriteLine(Path.GetFileNameWithoutExtension(file));
            }*/


            string songIndex = "";
            switch (mapName.Substring(0, mapName.IndexOf("-")))
            {
                case "MegaMan":
                    songIndex = "1";
                    break;
                case "MegaManII":
                    songIndex = "2";
                    break;
                case "MegaManIII":
                    songIndex = "3";
                    break;
                case "MegaManIV":
                    songIndex = "4";
                    break;
                case "MegaManV":
                    songIndex = "5";
                    break;
                case "MegaManVI":
                    songIndex = "6";
                    break;
                case "MegaMan9":
                    songIndex = "9";
                    break;
                case "MegaMan10":
                    songIndex = "10";
                    break;

            }
            int nsfIndex = 0;
            string songSearchTerm = "";
            string songFile = "Mega_Man_1.nsf";
            foreach (List<string> nsf in songList)
            {
                if (nsf[0] == "Mega_Man_" + songIndex + ".nsf")
                {
                    nsfIndex = songList.IndexOf(nsf);

                    songFile = nsf[0];
                    continue;
                }
            }


            string fileName = mapName;
            if (!noNameFixes)
            {
                if (!fileName.Contains("Wily") && !fileName.Contains("Wiley"))// && !fileName.Contains("MrX") && !fileName.Contains("ProtoMan") && !fileName.Contains("Cossack"))
                {

                    fileName = fileName.Replace("MegaMan10-", "")
                        .Replace("MegaMan9-", "")
                        .Replace("MegaManVI-", "")
                        .Replace("MegaManV-", "")
                        .Replace("MegaManIV-", "")
                        .Replace("MegaManIII-", "")
                        .Replace("MegaManII-", "")
                        .Replace("MegaMan-", "")
                        .Replace("MrX'sFortress-Level", "MrX")
                        .Replace("ProtoMan'sFortress-Level", "Dark")
                        .Replace("DrCossack'sCitadel-Level", "Cossack")
                        .Replace("Pharaoh", "Pharoah");
                    if (fileName.Contains("DocRobot-"))
                    {
                        fileName = fileName.Replace("Man", "").Replace("man", "").Replace("Revisited", "").Replace("DocRobot-", "Doc");
                    }
                }
                else
                {
                    fileName = fileName.Replace("WilyCastleStage-", "Wily").Replace("WilyCastle-Stage", "Wily").Replace("Wily'sCastle-Level", "Wily").Replace("DrWily-Level", "Wily").Replace("Wiley'sCastle-Level", "Wily");
                    fileName = fileName.Replace("MegaMan10-", "MM10")
                        .Replace("MegaMan9-", "MM9")
                        .Replace("MegaManVI-", "MM6")
                        .Replace("MegaManV-", "MM5")
                        .Replace("MegaManIV-", "MM4")
                        .Replace("MegaManIII-", "MM3")
                        .Replace("MegaManII-", "MM2")
                        .Replace("MegaMan-", "MM1");
                }
            }
            string levelNumber = "";
            if (fileName.Contains("MM"))
            {
                songSearchTerm = "Wily";
                levelNumber = fileName.Substring(fileName.IndexOf("Wily") + 4, 1);
            }
            else
            {
                songSearchTerm = fileName.Replace("Man", " Man");
            }

            string trackNumber = "-1";
            //string simp = "";
            foreach (string track in songList[nsfIndex])
            {
                if (levelNumber != "")
                {
                    if (track.Contains(songSearchTerm) && track.Substring(track.IndexOf('-'), track.Length - track.IndexOf('-')).Contains(levelNumber))
                    {
                        //simp = track.Substring(track.IndexOf('-'), track.Length - track.IndexOf('-'));
                        trackNumber = track.Substring(0, track.IndexOf(" "));
                        //int d = 0;
                        continue;
                    }
                }
                else
                {
                    if (track.Contains(songSearchTerm))
                    {

                        trackNumber = track.Substring(0, track.IndexOf(" "));//songList[nsfIndex].IndexOf(track);
                        continue;

                    }
                }
            }
            if (trackNumber == "-1")
            {
                int d = 0;
            }
            string songCC = string.Format("playMusic(\"{0}\",\"VGM\",{1},0,0,1,1);", songFile, trackNumber);
            string tileName = "tst" + fileName;//Console.ReadLine() + ".png";

            //Console.WriteLine("Provide output name:");
            string outputName = "lvl" + fileName;//Console.ReadLine();

            if (!overwriteExisting && File.Exists("rooms/" + outputName + ".room.gmx"))
            {
                Console.WriteLine(outputName + " already exists!");
                return mapName + " already exists!";
            }

            if (!File.Exists("maps/" + mapName.Replace("Animated1", "") + ".png"))
            {
                return mapName.Replace("Animated1", "") + " as a map does not exist!";
            }
            Bitmap map = Bitmap.FromFile("maps/" + mapName.Replace("Animated1","") + ".png") as Bitmap;
            string fileCheck = "tilesets/" + tileName + ".png";
            if (!File.Exists(fileCheck))
            {
                fileCheck = fileCheck.Replace("Man", "man");
                if (!File.Exists(fileCheck))
                {
                    Console.WriteLine(fileCheck + " as a tileset does not exist.");
                    return mapName + " as a tileset doesn't exist.";
                }
            }

            Bitmap tileset = Bitmap.FromFile(fileCheck) as Bitmap;


            


            if (map.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed || map.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || map.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {//If map is in index format, change to non-indexed.
             //using (Bitmap oldBmp = new Bitmap("myimage.jpg"))
                using (Bitmap newBmp = new Bitmap(map))
                using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    map = targetBmp.Clone() as Bitmap;// targetBmp is now in the desired format.
                }
            }
            if (tileset.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed || tileset.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || tileset.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {//If tileset is in index format, change to non-indexed.
             //using (Bitmap oldBmp = new Bitmap("myimage.jpg"))
                using (Bitmap newBmp = new Bitmap(tileset))
                using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    tileset = targetBmp.Clone() as Bitmap;// targetBmp is now in the desired format.
                }
            }
            tileset = BrightWhite(tileset);
            Color[] palette = tileset.Palette.Entries;


            bool doCutoff = false;
            bool doTop = false;
            bool nesMode = false;

            /*if (map.Height % 224 != 0)
            {
                doCutoff = true;
                Console.WriteLine("Cutoff on. Do top cutoff instead of bottom? y/n");
                if (Console.ReadLine() == "y")
                {
                    doTop = true;
                }
            }*/


            /*if (!mapName.Contains("Blade"))
            {
                return false;
            }*/





            if (palette.Length == 0)//If no palette table provided, make one ourselves.
            {
                List<Color> myColors = new List<Color>();
                for (var i = 0; i < tileset.Width; i++)
                {
                    for (var j = 0; j < tileset.Height; j++)
                    {
                        if (!myColors.Contains(tileset.GetPixel(i, j)) && tileset.GetPixel(i, j).A == 255)
                        {
                            myColors.Add(tileset.GetPixel(i, j));
                        }
                    }
                }
                palette = myColors.ToArray();
            }
            Console.WriteLine(mapName + ": Pallete Done.");
            int cutoffAmount = 0;
            map = BrightWhite(map);
            var ogMap = new Bitmap(map);//Before simplify so we keep mega man.



            //Task task_Megaman = Task.Run(() =>
            //{




            if (doManualNESMode)
            {
                nesMode = manualNESMode;

            }
            else
            {
                if ((map.Height - 32) % 224 != 0 && !mapName.Contains("MegaMan9") && !mapName.Contains("MegaMan10"))//Acount for the difference pre-crop.
                {
                    nesMode = true;
                    Console.WriteLine(mapName + ": Will output in NES mode. " + map.Height);
                }
                else
                {
                    Console.WriteLine(mapName + ": Will output in Wii mode. ");
                }
            }
            
            var megamanObj = "";

            if (!skipMega)
            {
                Console.WriteLine(mapName + ": Finding Mega Man...");
                Bitmap myMega;
                if (nesMode)
                {
                    myMega = new Bitmap(megamanImage);
                }
                else
                {
                    myMega = new Bitmap(megamanImage910);
                }

                megamanObj = MeasureMegaman(ogMap, myMega, SetInstanceName(ref rand, ref rand_CalledValues), mapName);
            }


            SimplifyMap(ref map, ref palette, doCutoff, doTop, cutoffAmount);
            ogMap = new Bitmap(map);//Now recopy after so we can be used for borders.
            Bitmap myBorderTiles;
            lock (borderTiles)
            { 
                myBorderTiles = new Bitmap(borderTiles);
            }
            SimplifyMap(ref myBorderTiles, ref palette, doCutoff, doTop, cutoffAmount);

            
            map = CropImage(map, new Rectangle(16, 16, map.Width - 32, map.Height - 32));

            
            //map
            if (indivMode)
            {
                map.Save("simpleMap.png");
            }



            
            //});


            Console.WriteLine(mapName + ": Map simplified. Creating room.");
            string roomData = File.ReadAllText("lvlCopyThisRoom.room.gmx");

            //roomData.Replace("<width>256</width>")
            //XmlDocument roomData = new XmlDocument();
            //roomData.LoadXml(File.ReadAllText("lvlCopyThisRoom.room.gmx"));
            //roomData.GetElementsByTagName("width")[0] = map.Width.ToString();
            //roomData.SelectSingleNode("room/height").Value = map.Height.ToString();
            roomData = roomData.Replace("<width>256</width>", "<width>" + map.Width + "</width>");
            roomData = roomData.Replace("<height>224</height>", "<height>" + map.Height + "</height>");
            if (nesMode)
            {
                roomData = roomData.Replace("bgQuadMM9_10", "bgQuadMM1_6");
            }

            string tileSetName = tileName;
            int tileID = 11539893;



            string tiles = "";
            bool isBlankScreen = false;

            string objectSetterObject = "";
            int nesValue = 224;
            if (nesMode)
            {
                nesValue = 240;
            }
            //bool[,] blankRegistry = new bool[map.Width / 256, map.Height / nesValue];
            string borderObjects = "";


            int genSuccess = 0;
            int genFail = 0;

            for (int i = 0; i < map.Width; i += tsaValue)
            {
                for (int j = 0; j < map.Height; j += tsaValue)
                {
                    if (i % 256 == 0 && j % nesValue == 0)
                    {
                        if (i != 0)
                        {
                            if (objectSetterObject == "" && isBlankScreen == true)
                            {
                                string instID = SetInstanceName(ref rand, ref rand_CalledValues);//FIX THIS TO PREVENT REPEATS LATER. MAY BE THE CAUSE OF TILE BUG.
                                                                                                                          //<tile bgName="tstArena_Base" x="0" y="48" w="16" h="16" xo="64" yo="0" id="11539893" name="inst_CE27C1DD" depth="1700000" locked="0" colour="4294967295" scaleX="1" scaleY="1"/>
                                                                                                                          //<tile bgName=\"{0}\" x=\"{1}\" y=\"{2}\" w=\"16\" h=\"16\" xo=\"64\" yo=\"0\" id=\"{3}\" name=\"inst_{4}\" depth=\"1000000\" locked=\"0\" colour=\"4294967295\" scaleX=\"1\" scaleY=\"1\"/>
                                tiles += string.Format("\t\t<tile bgName=\"{0}\" x=\"{1}\" y=\"{2}\" w=\"" + tileset.Width + "\" h=\"" + tileset.Height + "\" xo=\"{3}\" yo=\"{4}\" id=\"{5}\" name=\"inst_{6}\" depth=\"1000000\" locked=\"0\" colour=\"4294967295\" scaleX=\"1\" scaleY=\"1\"/>\n", tileSetName, i - 256, j - 224, 0, 0, tileID, instID);
                                instID = SetInstanceName(ref rand, ref rand_CalledValues);
                                objectSetterObject = string.Format("<instance objName=\"{0}\" x=\"{1}\" y=\"{2}\" name=\"inst_{3}\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", "objObjectSetter", i - 256, j - 224, instID);
                                tileID++;
                            }

                            isBlankScreen = true;
                        }

                        //Task[] tileTasks = new Task[2];
                        //tileTasks[0] = Task.Run(() =>
                        //{
                        if (!skipBorders)
                        {
                            bool isBorderTile = false;
                            bool horiBorder = false;
                            for (int k = 0; k < myBorderTiles.Width; k += 16)
                            {
                                for (int l = 0; l < myBorderTiles.Height; l += 16)
                                {


                                    if (true)//k < myBorderTiles.Width && l < myBorderTiles.Height && k % 16 == 0 && l % 16 == 0)//Do this when tsaValue acts as though it were 16 to avoid having to have two kl loops.
                                    {//Note: Will not work right if map is larger than bordertile image. Shouldn't really be an issue though.
                                        if (MeasureTile(map, myBorderTiles, i, j, k, l, 16, 0))//Border on the right (On top of us)
                                        {

                                            if ((k == 48 && l == 32) || (k == 0 && l == 0))//((l == 0 || l == 64) && (k == 0 || k == 64)))
                                            {
                                                if (!horiBorder)
                                                {
                                                    borderObjects += CreateBorderObject(true, i, j, SetInstanceName(ref rand, ref rand_CalledValues));

                                                    horiBorder = true;
                                                }
                                                isBorderTile = true;
                                            }
                                            if (j >= 16)
                                            {
                                                /*bool doVScroll = true;
                                                for (int a = 0; a < myBorderTiles.Width; a += 16)
                                                {
                                                    for (int b = 0; b < myBorderTiles.Height; b += 16)
                                                    {

                                                    }
                                                }
                                                if (doVScroll)
                                                {
                                                    borderObjects += CreateBorderObject(false, i, j, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));
                                                }*/
                                            }
                                        }
                                        if (MeasureTile(ogMap, myBorderTiles, i /*+ 16*/, j + 16, k, l, 16, 0))// Area to the left of us on main map.|| MeasureTile(ogMap, myBorderTiles, i + 16, j+16, a, b, 16))//MeasureTile(ogMap, myBorderTiles, i, j - 32, 16, 16, 16) && !MeasureTile(ogMap, myBorderTiles, i, j - 32, 32, 32, 16))
                                        {
                                            if (((k == 16 && l == 32) && !MeasureTile(ogMap, myBorderTiles, i, j, 16, 16, 16, 0)) || (k == 64 && l == 0))
                                            {

                                                borderObjects += CreateBorderObject(false, i, j, SetInstanceName(ref rand, ref rand_CalledValues));
                                                if (!horiBorder)
                                                {
                                                    borderObjects += CreateBorderObject(true, i, j, SetInstanceName(ref rand, ref rand_CalledValues));
                                                    horiBorder = true;
                                                }
                                                isBorderTile = true;

                                            }

                                            //doVScroll = false;
                                            //a = myBorderTiles.Width;
                                            //b = myBorderTiles.Height;
                                        }
                                        else if (MeasureTile(ogMap, myBorderTiles, i /*+ 16*/, j, k, l, 16, 0))//If both current and behind are occupied, check above the one behind us to see if we're a start of a corner up.
                                        {
                                            if (k == 64 && l == 64)
                                            {
                                                borderObjects += CreateBorderObject(false, i, j, SetInstanceName(ref rand, ref rand_CalledValues));
                                                isBorderTile = true;
                                            }
                                        }
                                        /*var ogOffset = 0;
                                        if (i == 0)
                                        {
                                            ogOffset = 16;
                                        }*/


                                        if (i != 0 && MeasureTile(ogMap, myBorderTiles, i, j + 16, k, l, 16, 0))//Border on the left (behind us), adjusted for ogmap.
                                        {

                                            if (((k == 16 && l == 32) || (k == 64 && l == 0)))
                                            {
                                                if (!horiBorder)
                                                {
                                                    borderObjects += CreateBorderObject(true, i, j, SetInstanceName(ref rand, ref rand_CalledValues));
                                                    horiBorder = true;
                                                }
                                                isBorderTile = true;
                                            }

                                            /*if ((k == 16 && l == 16) == false)
                                            {
                                                if (l <= 16)
                                                {

                                                    if (k == 0)
                                                    {
                                                        borderObjects += CreateBorderObject(true, i, j, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));

                                                    }
                                                    else if (k == 32)
                                                    {

                                                        borderObjects += CreateBorderObject(true, i - 16, j, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));
                                                    }
                                                    if (MeasureTile(map, myBorderTiles, i, j - 16, k, l - 16, 16))
                                                    {
                                                        borderObjects += CreateBorderObject(false, i, j, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));
                                                    }
                                                }
                                            }*/
                                        }
                                        /*else
                                        { 

                                        }*/
                                        /*if (k == 1 && l == 0)
                                        {
                                            borderObjects += CreateBorderObject(true, i, j+16, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));
                                        }
                                        if (k == 1 && l == 2)
                                        {
                                            borderObjects += CreateBorderObject(true, i, j-16, rand.Next(0, 0xFFFF).ToString("X4") + rand.Next(0, 0xFFFF).ToString("X4"));
                                        }*/
                                    }

                                }
                            }
                            if (!isBorderTile)
                            {
                                bool doVert = true;
                                for (int k = 0; k < myBorderTiles.Width; k += 16)
                                {
                                    for (int l = 0; l < myBorderTiles.Height; l += 16)
                                    {
                                        if (MeasureTile(ogMap, myBorderTiles, i + 16, j, k, l, 16, 0) || MeasureTile(ogMap, myBorderTiles, i + 16, j + 16, k, l, 16, 0))//Above us or on us.
                                        {
                                            doVert = false;
                                            k = myBorderTiles.Width;
                                            l = myBorderTiles.Height;
                                        }
                                    }
                                }
                                if (doVert)
                                {
                                    borderObjects += CreateBorderObject(false, i, j, SetInstanceName(ref rand, ref rand_CalledValues));
                                }
                            }
                        }
                    }
                    //});
                    //tileTasks[1] = Task.Run(() =>
                    //{
                    if (!IsBlackTile(map, i, j, tsaValue))
                    {
                        bool gotTile = false;
                        for (int k = 0; k < tileset.Width; k += tsaValue)
                        {
                            for (int l = 0; l < tileset.Height; l += tsaValue)
                            {
                                if (MeasureTile(map, tileset, i, j, k, l, tsaValue))
                                {

                                    if (!IsBlankTile(tileset, k, l, tsaValue))
                                    {
                                        string instID = SetInstanceName(ref rand, ref rand_CalledValues);//FIX THIS TO PREVENT REPEATS LATER. MAY BE THE CAUSE OF TILE BUG.
                                                                                                                                  //<tile bgName="tstArena_Base" x="0" y="48" w="16" h="16" xo="64" yo="0" id="11539893" name="inst_CE27C1DD" depth="1700000" locked="0" colour="4294967295" scaleX="1" scaleY="1"/>
                                                                                                                                  //<tile bgName=\"{0}\" x=\"{1}\" y=\"{2}\" w=\"16\" h=\"16\" xo=\"64\" yo=\"0\" id=\"{3}\" name=\"inst_{4}\" depth=\"1000000\" locked=\"0\" colour=\"4294967295\" scaleX=\"1\" scaleY=\"1\"/>
                                        tiles += string.Format("\t\t<tile bgName=\"{0}\" x=\"{1}\" y=\"{2}\" w=\"" + tsaValue + "\" h=\"" + tsaValue + "\" xo=\"{3}\" yo=\"{4}\" id=\"{5}\" name=\"inst_{6}\" depth=\"1000000\" locked=\"0\" colour=\"4294967295\" scaleX=\"1\" scaleY=\"1\"/>\n", tileSetName, i, j, k, l, tileID, instID);
                                        k = tileset.Width;
                                        l = tileset.Height;
                                        tileID++;
                                        isBlankScreen = false;
                                        genSuccess++;
                                        gotTile = true;
                                    }
                                }
                            }

                        }
                        if (gotTile == false && (tsaValue == 16 || (i + 16 < map.Width && j+16 < map.Height)))//!IsBlackTile(map, i, j, 16)))
                        {
                            bool isBorderTile = false;
                            for (int k = 0; k < myBorderTiles.Width; k += 16)
                            {
                                for (int l = 0; l < myBorderTiles.Height; l += 16)
                                {
                                    if (MeasureTile(map, myBorderTiles, i, j, k, l, 16))//Border on the right (On top of us)
                                    {
                                        isBorderTile = true;
                                    }
                                }
                            }
                            if (!isBorderTile)
                            {
                                genFail++;
                            }
                        }
                    }
                    /*else
                    {
                        genSuccess++;
                    }*/
                    //});
                    //await Task.WhenAll(tileTasks);
                }

            }
            double ratio = (double)genSuccess / (double)(genFail+genSuccess);
            

            //objectSetterObject = objectSetterObject;
            

            roomData = roomData.Replace("<tiles/>", "<tiles>\n" + tiles + "</tiles>");
            roomData = roomData.Replace("<code></code>", "<code>" + songCC + "</code>");

            //await task_Megaman;
            if (megamanObj != "")
            {
                roomData = roomData.Replace("<instance objName=\"objDefaultSpawn\" x=\"128\" y=\"144\" name=\"inst_D3E2D883\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", "");
            }
            roomData = roomData.Replace("    <instance objName=\"objSolid\" x=\"112\" y=\"160\" name=\"inst_DD6DCD32\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\r\n    <instance objName=\"objSolid\" x=\"128\" y=\"160\" name=\"inst_51923CF9\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\r\n    <instance objName=\"objSolid\" x=\"144\" y=\"160\" name=\"inst_07774C3D\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\r\n    <instance objName=\"objSolid\" x=\"96\" y=\"160\" name=\"inst_5A9CB10F\" locked=\"0\" code=\"visible = true;&#xA;\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", "");
            roomData = roomData.Replace("    <instance objName=\"objSolid\" x=\"112\" y=\"160\" name=\"inst_DD6DCD32\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\n    <instance objName=\"objSolid\" x=\"128\" y=\"160\" name=\"inst_51923CF9\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\n    <instance objName=\"objSolid\" x=\"144\" y=\"160\" name=\"inst_07774C3D\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>\n    <instance objName=\"objSolid\" x=\"96\" y=\"160\" name=\"inst_5A9CB10F\" locked=\"0\" code=\"visible = true;&#xA;\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", "");
           


            roomData = roomData.Replace("</instances>", "\t" + megamanObj + "\t" + objectSetterObject + "\t" + borderObjects + "\r\n\t</instances>");
            File.WriteAllText("rooms/" + outputName + ".room.gmx", roomData);
            //roomData.Save("outroom.txt");


            Console.WriteLine("Done: " + mapName);
            string megaOut = "";
            if (megamanObj == "" && !skipMega)
            {
                megaOut = ", Couldn't find MM";
            }
            return mapName + " Tiles found: " + genSuccess + "/" + (genFail+genSuccess) + ", " + Math.Round(ratio * 100, 2) + "%" + megaOut;
        }

        
        static bool MeasureTile(Bitmap map, Bitmap tileset, int x1, int y1, int x2, int y2, int tsaValue, int deltaERatio = 10)
        {
            //var isEqual = true;
            for (var i = 0; i < tsaValue; i++)
            {
                for (int j = 0; j < tsaValue; j++)
                {

                    bool notEqual = false;

                    if (estimateColors)
                    {
                        Color mapColor = map.GetPixel(x1 + i, y1 + j);
                        Color tileColor = tileset.GetPixel(x2 + i, y2 + j);
                        var deltaE = ColorFormulas.DoFullCompare(mapColor.R, mapColor.G, mapColor.B, tileColor.R, tileColor.G, tileColor.B);
                        notEqual = deltaE > deltaERatio;
                    }
                    else
                    {
                        notEqual = map.GetPixel(x1 + i, y1 + j) != tileset.GetPixel(x2 + i, y2 + j);
                    }


                    if (notEqual && tileset.GetPixel(x2 + i, y2 + j).A != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static string MeasureMegaman(Bitmap map, Bitmap megaman, string instanceID, string mapName)
        {
            //var isEqual = true;
            //Parallel.For(0, map.Width/*, new ParallelOptions { MaxDegreeOfParallelism = 20 }*/, i =>
            //{
            //Parallel.For(0, map.Height/*, new ParallelOptions { MaxDegreeOfParallelism = 20 }*/, j =>
            //{
            for (int i = 0; i < map.Width-megaman.Width; i++)
            {
                for (int j = 0; j < map.Height-megaman.Height; j++)
                { 
                    bool complete = true;
                    for (int k = 0; k < megaman.Width; k++)
                    {
                        for (int l = 0; l < megaman.Height; l++)
                        {
                            var mmPix = megaman.GetPixel(k, l);

                            if (mmPix.A != 0) //map.GetPixel(i + k, j + l) != megaman.GetPixel(k, l))
                            {
                                var mapPix = map.GetPixel(i + k, j + l);
                                var deltaE = ColorFormulas.DoFullCompare(mapPix.R, mapPix.G, mapPix.B, mmPix.R, mmPix.G, mmPix.B);
                                if (deltaE > 25)
                                {
                                    k = megaman.Width;
                                    l = megaman.Height;
                                    complete = false;
                                }
                            }
                        }
                    }
                    if (complete)
                    {
                        Console.WriteLine(mapName + ": Found Mega Man.");
                        return string.Format("<instance objName=\"{0}\" x=\"{1}\" y=\"{2}\" name=\"inst_{3}\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", "objDefaultSpawn", i-16+11, j-16+7, instanceID);
                    }

                }//);
            }//);
            var def = Console.ForegroundColor;
            Console.ForegroundColor = System.ConsoleColor.Red;
           
            Console.WriteLine(mapName + ": Could not find Megaman!");
            Console.ForegroundColor = def;
            return "";
        }
        static bool IsBlackTile(Bitmap map, int x1, int y1, int tsaValue)
        {
            for (var i = 0; i < tsaValue; i++)
            {
                for (int j = 0; j < tsaValue; j++)
                {
                    Color col = map.GetPixel(x1+i, y1+j);
                    if (col.R + col.G + col.B > 20)//Treat as not black
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static bool IsBlankTile(Bitmap map, int x1, int y1, int tsaValue)
        {
            for (var i = 0; i < tsaValue; i++)
            {
                for (int j = 0; j < tsaValue; j++)
                {
                    Color col = map.GetPixel(x1 + i, y1 + j);
                    if (col.A > 0)//Treat as not blank
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static bool IsBlackOrWhiteTile(Bitmap map, int x1, int y1, int tsaValue)
        {
            for (var i = 0; i < tsaValue; i++)
            {
                for (int j = 0; j < tsaValue; j++)
                {
                    Color col = map.GetPixel(x1 + i, y1 + j);
                    if (col.R + col.G + col.B > 20 || (col.R + col.G + col.B >= 744))//Treat as not black
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static bool CutoffCriteria(int j, bool doTop)
        {
            if (doTop)
            {
                return ((j) % (224)) == 0;
            }
            else
            {
                return ((j) % (14 * 16)) == 0;
            }
        }
        private static Bitmap CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat) as Bitmap;
        }

        private static Bitmap BrightWhite(Bitmap img)
        {
            for (var i = 0; i < img.Width; i++)
            {
                for (var j = 0; j < img.Height; j++)
                {
                    Color c = img.GetPixel(i, j);
                    if (c.A != 0 && c.R + c.G + c.B >= 744)//248 248 248
                    {
                        img.SetPixel(i, j, Color.White);
                    }
                    if (c.A != 0 && c.R + c.G + c.B <= 15)
                    {
                        img.SetPixel(i, j, Color.Black);
                    }
                }
            }
            return img;
        }


        private static void SimplifyMap(ref Bitmap map, ref Color[] palette, bool doCutoff, bool doTop, int cutoffAmount)
        {
            for (var j = 0; j < map.Height; j++)//Y then X so we can make the 16 cutoff easier.
            {
                if (doCutoff && CutoffCriteria(j, doTop))
                {
                    cutoffAmount += 16;
                }
                for (var i = 0; i < map.Width; i++)
                {

                    Color oldColor;
                    Color correctValue;
                    if (j + cutoffAmount >= map.Height)
                    {
                        oldColor = Color.Black;
                        correctValue = Color.Black;
                    }
                    /*else if (CutoffCriteria(j,doTop))//Debugging. Will cause map to fail on top pixels when active though.
                    {
                        oldColor = Color.Red;
                        correctValue = Color.Red;
                    }*/
                    else
                    {
                        //continue;
                        oldColor = map.GetPixel(i, j + cutoffAmount);
                        correctValue = map.GetPixel(i, j + cutoffAmount);
                        //if (Array.IndexOf(palette, oldColor) < 0)
                        //{
                        //for (int i = 0)
                        if (oldColor.A == 255)
                        {
                            double minimumDelta = 999;
                            //var deltaE2 = ColorFormulas.DoFullCompare(230,230,230,255,255,255);
                            //var deltaE3 = ColorFormulas.DoFullCompare(255, 255, 255, 255, 255, 255);
                            for (var k = 0; k < palette.Length; k++)
                            {
                                if (ColorCheck(oldColor, palette[k]))//oldColor == palette[k])
                                {
                                    correctValue = palette[k];
                                    k = palette.Length;
                                }
                                else
                                {

                                    var deltaE = ColorFormulas.DoFullCompare(oldColor.R, oldColor.G, oldColor.B, palette[k].R, palette[k].G, palette[k].B);//oldColor
                                    if (/*deltaE < 10 &&*/deltaE < minimumDelta)
                                    {
                                        correctValue = palette[k];
                                        minimumDelta = deltaE;
                                    }
                                    if (deltaE == 0)
                                    {
                                        k = palette.Length;
                                    }
                                }
                            }
                            if (oldColor != correctValue)
                            {
                                int d = 0;
                            }

                        }
                        //}
                    }

                    map.SetPixel(i, j, correctValue);//If you crash here, give the image more colors!
                                                     //map.SetPixel(i, j, Color.White);

                }

            }
        }
        private static string CreateBorderObject(bool hori, int xPos, int yPos, string instanceID)
        {
            string name = "objStopScrollingHorizontal";
            if (!hori)
            {
                name = "objVerticalScrolling";
            }
            return string.Format("<instance objName=\"{0}\" x=\"{1}\" y=\"{2}\" name=\"inst_{3}\" locked=\"0\" code=\"\" scaleX=\"1\" scaleY=\"1\" colour=\"4294967295\" rotation=\"0\"/>", name, xPos, yPos, instanceID);
        }
        private static bool ColorCheck(Color col1, Color col2)
        {
            if (col1.R == col2.R && col1.G == col2.G && col1.B == col2.B && col1.A == col2.A)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static uint RandUniqueNext(ref Random rand, ref List<uint> calledValues, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {

            uint output = unchecked((uint)(rand.Next(minValue, maxValue)));
            while (calledValues.IndexOf(output) != -1)
            {
                output = (uint)(rand.Next((int)(minValue - 0xFFFF), (int)(maxValue - 0xFFFE)) + 0xFFFF);
            }
            calledValues.Add(output);
            return output;
        }
        static string SetInstanceName(ref Random rand, ref List<uint> calledValues)
        {

            return RandUniqueNext(ref rand, ref calledValues).ToString("X8");
            //SetInstanceName(ref rand, ref rand_CalledValues)
        }
    }

    
    

    public class ColorFormulas
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double CieL { get; set; }
        public double CieA { get; set; }
        public double CieB { get; set; }

        public ColorFormulas(int R, int G, int B)
        {
            RGBtoLAB(R, G, B);
        }

        public void RGBtoLAB(int R, int G, int B)
        {
            RGBtoXYZ(R, G, B);
            XYZtoLAB();
        }

        public void RGBtoXYZ(int RVal, int GVal, int BVal)
        {
            double R = Convert.ToDouble(RVal) / 255.0;       //R from 0 to 255
            double G = Convert.ToDouble(GVal) / 255.0;       //G from 0 to 255
            double B = Convert.ToDouble(BVal) / 255.0;       //B from 0 to 255

            if (R > 0.04045)
            {
                R = Math.Pow(((R + 0.055) / 1.055), 2.4);
            }
            else
            {
                R = R / 12.92;
            }
            if (G > 0.04045)
            {
                G = Math.Pow(((G + 0.055) / 1.055), 2.4);
            }
            else
            {
                G = G / 12.92;
            }
            if (B > 0.04045)
            {
                B = Math.Pow(((B + 0.055) / 1.055), 2.4);
            }
            else
            {
                B = B / 12.92;
            }

            R = R * 100;
            G = G * 100;
            B = B * 100;

            //Observer. = 2°, Illuminant = D65
            X = R * 0.4124 + G * 0.3576 + B * 0.1805;
            Y = R * 0.2126 + G * 0.7152 + B * 0.0722;
            Z = R * 0.0193 + G * 0.1192 + B * 0.9505;
        }

        public void XYZtoLAB()
        {
            // based upon the XYZ - CIE-L*ab formula at easyrgb.com (http://www.easyrgb.com/index.php?X=MATH&H=07#text7)
            double ref_X = 95.047;
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double var_X = X / ref_X;         // Observer= 2°, Illuminant= D65
            double var_Y = Y / ref_Y;
            double var_Z = Z / ref_Z;

            if (var_X > 0.008856)
            {
                var_X = Math.Pow(var_X, (1 / 3.0));
            }
            else
            {
                var_X = (7.787 * var_X) + (16 / 116.0);
            }
            if (var_Y > 0.008856)
            {
                var_Y = Math.Pow(var_Y, (1 / 3.0));
            }
            else
            {
                var_Y = (7.787 * var_Y) + (16 / 116.0);
            }
            if (var_Z > 0.008856)
            {
                var_Z = Math.Pow(var_Z, (1 / 3.0));
            }
            else
            {
                var_Z = (7.787 * var_Z) + (16 / 116.0);
            }

            CieL = (116 * var_Y) - 16;
            CieA = 500 * (var_X - var_Y);
            CieB = 200 * (var_Y - var_Z);
        }

        ///
        /// The smaller the number returned by this, the closer the colors are
        ///
        ///
        /// 
        public double CompareTo(ColorFormulas oComparisionColor)
        {
            // Based upon the Delta-E (1976) formula at easyrgb.com (http://www.easyrgb.com/index.php?X=DELT&H=03#text3)
            double DeltaE = Math.Sqrt(Math.Pow((CieL - oComparisionColor.CieL), 2) + Math.Pow((CieA - oComparisionColor.CieA), 2) + Math.Pow((CieB - oComparisionColor.CieB), 2));
            return DeltaE;//Convert.ToInt16(Math.Round(DeltaE));
        }

        public static double DoFullCompare(int R1, int G1, int B1, int R2, int G2, int B2)
        {
            ColorFormulas oColor1 = new ColorFormulas(R1, G1, B1);
            ColorFormulas oColor2 = new ColorFormulas(R2, G2, B2);
            return oColor1.CompareTo(oColor2);
        }

    }
}
