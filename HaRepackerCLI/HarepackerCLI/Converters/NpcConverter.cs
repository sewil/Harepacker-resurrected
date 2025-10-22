using MapleLib.WzLib;
using MapleLib.WzLib.Serializer;
using MapleLib.WzLib.WzProperties;

namespace HarepackerCLI.Converters
{
    internal class NpcConverter
    {
        public static void Convert(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("Usage: HarepackerCLI.exe convert npc <data-directory> <game-version> <from-data-directory> <npc-id>");
                Environment.Exit(1);
            }
            else
            {
                string toDataDirectory = args[2];
                string gameVersion = args[3];
                string fromDataDirectory = args[4];
                int npcID = int.Parse(args[5]);
                WzFile npcWz = new WzFile(Path.Join(fromDataDirectory, "Npc.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(npcWz);
                WzFile stringWz = new WzFile(Path.Join(fromDataDirectory, "String.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(stringWz);
                WzFile toDataWz = new WzFile(Path.Join(toDataDirectory, "Data.wz"), gameVersion, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(toDataWz);

                var serializer = new WzImgSerializer();
                var deserializer = new WzImgDeserializer(true);

                // Convert img
                var npcImg = (WzImage)npcWz[npcID.ToString().PadLeft(7, '0') + ".img"];
                var npcInfo = npcImg["info"];

                if (npcInfo["script"] != null)
                {
                    string scriptName = npcInfo["script"]["0"]["script"].GetString();
                    npcInfo["script"].Remove();
                    npcInfo.WzProperties.Add(new WzStringProperty("quest", scriptName));
                }

                var toNpcDir = (WzDirectory)toDataWz["Npc"];
                toNpcDir.AddImage(npcImg);

                // Export img
                npcImg.Changed = true;
                string npcImgFilePath = Path.Join(toDataDirectory, "Npc", npcImg.Name);
                if (File.Exists(npcImgFilePath)) File.Copy(npcImgFilePath, npcImgFilePath + ".bak", true);
                serializer.SerializeImage(npcImg, npcImgFilePath);
                Console.WriteLine("Created file " + npcImgFilePath);

                // Convert string
                var npcString = (WzImageProperty)stringWz["Npc.img"][npcID.ToString()];
                string toNpcStrImgFilePath = Path.Join(toDataDirectory, "String", "Npc.img");
                var toNpcStrImg = deserializer.WzImageFromIMGFile(toNpcStrImgFilePath, toDataWz.WzIv, "Npc.img", out bool success);
                toDataWz.WzDirectory.AddImage(toNpcStrImg);
                toNpcStrImg.AddProperty(npcString);

                // Export string
                toNpcStrImg.Changed = true;
                File.Copy(toNpcStrImgFilePath, toNpcStrImgFilePath + ".bak", true);
                serializer.SerializeImage(toNpcStrImg, toNpcStrImgFilePath);
                Console.WriteLine("Updated file " + toNpcStrImgFilePath);
            }
        }
    }
}
