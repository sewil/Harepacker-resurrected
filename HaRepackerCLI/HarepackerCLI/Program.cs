using HarepackerCLI.Converters;
using MapleLib;
using MapleLib.WzLib;
using MapleLib.WzLib.Serializer;

namespace HarepackerCLI
{
    internal class Program
    {
        static WzImgDeserializer imgDeserializer;
        static WzXmlDeserializer xmlDeserializer;
        static void Main(string[] args)
        {
            if (args.Length == 0 || (args[0] != "import" && args[0] != "convert"))
            {
                Console.WriteLine("Usage: HarepackerCLI.exe <import|convert> [options]");
                Environment.Exit(1);
            }
            else if (args[0] == "import")
            {
                if (args.Length < 5)
                {
                    Console.WriteLine("Usage: HarepackerCLI.exe import <game-version> <input directory> <output file> <packignore.txt>");
                    Environment.Exit(1);
                }
                else
                {
                    string gameVersion = args[1];
                    string folder = args[2];
                    string outPath = args[3];
                    string[] packIgnore = File.ReadAllLines(args[4]);
                    var mapleVersion = WzMapleVersion.CLASSIC;

                    var wzf = new WzFile(mapleVersion, gameVersion);
                    wzf.Header.Copyright = "Package file v1.0 Copyright 2002 Wizet, ZMS";
                    wzf.Header.RecalculateFileStart();
                    wzf.Name = "Data.wz";
                    wzf.WzDirectory.Name = "Data.wz";
                    imgDeserializer = new WzImgDeserializer(true);
                    xmlDeserializer = new WzXmlDeserializer(true, wzf.WzIv);
                    ImportFolder(wzf, wzf, folder, packIgnore);

                    wzf.SaveToDisk(outPath, null, mapleVersion);
                }
            }
            else if (args[0] == "convert")
            {
                if (args.Length < 4)
                {
                    Console.WriteLine(args.Length);
                    Console.WriteLine("Usage: HarepackerCLI.exe convert <map|quest|npc|reactor|mob> <data-directory> <game-version> [options]");
                    Environment.Exit(1);
                }
                switch (args[1])
                {
                    case "map":
                        MapConverter.Convert(args);
                        break;
                    case "quest":
                        QuestConverter.Convert(args);
                        break;
                    case "npc":
                        NpcConverter.Convert(args);
                        break;
                    case "mob":
                        MobConverter.Convert(args);
                        break;
                    case "reactor":
                        ReactorConverter.Convert(args);
                        break;
                    default:
                        Console.WriteLine("Unknown convert option \"" + args[1] + "\". Please use one of: map, quest, npc, reactor, mob.");
                        break;
                }
            }
        }

        static int ImportFolder(WzFile wzFile, WzObject parent, string folder, string[] packIgnore)
        {
            var subfolders = Directory.GetDirectories(folder)
                .Where(folder => !packIgnore.Contains(Path.GetFileName(folder)));

            foreach (var subfolder in subfolders)
            {
                string dirName = new DirectoryInfo(subfolder).Name;
                var subparent = new WzDirectory(dirName, wzFile);
                AddObj(parent, subparent);
                int importedObjects = ImportFolder(wzFile, subparent, subfolder, packIgnore);
                Console.WriteLine($"Imported directory '{dirName}' from '{subfolder}' with {importedObjects} objects");
            }

            var files = Directory.GetFiles(folder)
                .Where(file => !packIgnore.Contains(Path.GetFileName(file)));

            int objects = 0;

            foreach (string file in files)
            {
                try
                {
                    List<WzObject> objs = ParseFile(file, wzFile);
                    if (objs == null) continue;
                    foreach (WzObject obj in objs)
                    {
                        AddObj(parent, obj);
                        //Console.WriteLine($"Added object {obj.Name} for file '{file}'");
                        objects++;
                    }
                }
                catch (ThreadAbortException)
                {
                    return 0;
                }
            }
            return objects;
        }
        static void TryParseImage(WzObject obj)
        {
            if (obj is WzImage)
            {
                ((WzImage)obj).ParseImage();
            }
        }
        public static bool CanObjBeInserted(WzObject parent, string name)
        {
            if (parent is IPropertyContainer container)
                return container[name] == null;
            else if (parent is WzDirectory directory)
                return directory[name] == null;
            else if (parent is WzFile file)
                return file.WzDirectory?[name] == null;
            else
                return false;
        }
        static bool AddObj(WzObject parent, WzObject obj)
        {
            if (CanObjBeInserted(parent, obj.Name))
            {
                TryParseImage(obj);
                AddObjInternal(parent, obj);
                return true;
            }
            else
            {
                Console.WriteLine("Cannot insert node \"" + obj.Name + "\" because a node with the same name already exists. Skipping.");
                return false;
            }
        }
        static bool AddObjInternal(WzObject parent, WzObject obj)
        {
            if (parent is WzFile file)
                parent = file.WzDirectory;

            if (parent is WzDirectory directory)
            {
                if (obj is WzDirectory wzDirectory)
                    directory.AddDirectory(wzDirectory);
                else if (obj is WzImage wzImgProperty)
                    directory.AddImage(wzImgProperty);
                else
                    return false;
            }
            else if (parent is WzImage wzImageProperty)
            {
                if (!wzImageProperty.Parsed)
                    wzImageProperty.ParseImage();
                if (obj is WzImageProperty imgProperty)
                {
                    wzImageProperty.AddProperty(imgProperty);
                    wzImageProperty.Changed = true;
                }
                else
                    return false;
            }
            else if (parent is IPropertyContainer container)
            {
                if (obj is WzImageProperty property)
                {
                    container.AddProperty(property);
                    if (parent is WzImageProperty imgProperty)
                        imgProperty.ParentImage.Changed = true;
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        static List<WzObject> ParseFile(string file, WzFile wzFile)
        {
            try
            {
                if (file.ToLower().EndsWith(".img.xml"))
                {
                    return xmlDeserializer.ParseXML(file);
                }
                else if (file.ToLower().EndsWith(".img"))
                {
                    var objs = new List<WzObject>
                        {
                            imgDeserializer.WzImageFromIMGFile(file, wzFile.WzIv, Path.GetFileName(file), out bool successfullyParsedImage)
                        };

                    if (!successfullyParsedImage)
                    {
                        Console.WriteLine(string.Format("Error importing {0} file. Are you sure you have selected the correct WZ encryption?", file));
                        return null;
                    }
                    else
                    {
                        return objs;
                    }
                }
                else
                {
                    Console.WriteLine($"Unrecognized file \"{file}\", skipping...");
                    return null;
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("The file \"{0}\" is invalid and will be skipped. Error: {1}", file, e.Message));
                return null;
            }
        }
    }
}
