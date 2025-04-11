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
            string gameVersion = args[0];
            string folder = args[1];
            string outPath = args[2];
            string[] ignoredFolders = args[3].Split(";");
            var mapleVersion = WzMapleVersion.CLASSIC;
            var allowedExtensions = new[] { ".img", ".img.xml" };

            var wzf = new WzFile(mapleVersion, gameVersion);
            wzf.Header.Copyright = "Package file v1.0 Copyright 2002 Wizet, ZMS";
            wzf.Header.RecalculateFileStart();
            wzf.Name = "Data.wz";
            wzf.WzDirectory.Name = "Data.wz";
            imgDeserializer = new WzImgDeserializer(true);
            xmlDeserializer = new WzXmlDeserializer(true, wzf.WzIv);
            ImportFolder(wzf, wzf, folder, allowedExtensions, ignoredFolders);

            wzf.SaveToDisk(outPath, null, mapleVersion);
        }
        static int ImportFolder(WzFile wzFile, WzObject parent, string folder, string[] allowedExtensions, string[] ignoredFolders)
        {
            var subfolders = Directory.GetDirectories(folder)
                .Where(folder => !ignoredFolders.Contains(Path.GetFileName(folder)));

            foreach (var subfolder in subfolders)
            {
                string dirName = new DirectoryInfo(subfolder).Name;
                var subparent = new WzDirectory(dirName, wzFile);
                AddObj(parent, subparent);
                int importedObjects = ImportFolder(wzFile, subparent, subfolder, allowedExtensions, ignoredFolders);
                Console.WriteLine($"Imported directory '{dirName}' for folder '{subfolder}' with {importedObjects} objects");
            }

            var files = Directory
                .GetFiles(folder)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            int objects = 0;

            foreach (string file in files)
            {
                List<WzObject> objs;
                try
                {
                    if (file.ToLower().EndsWith(".img.xml"))
                    {
                        objs = xmlDeserializer.ParseXML(file);
                    }
                    else if (file.ToLower().EndsWith(".img"))
                    {
                        objs = new List<WzObject>
                        {
                            imgDeserializer.WzImageFromIMGFile(file, wzFile.WzIv, Path.GetFileName(file), out bool successfullyParsedImage)
                        };

                        if (!successfullyParsedImage)
                        {
                            Console.WriteLine(string.Format("Error importing {0} file. Are you sure you have selected the correct WZ encryption?", file));
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unrecognized file \"{file}\", skipping...");
                        continue;
                    }
                }
                catch (ThreadAbortException)
                {
                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("The file \"{0}\" is invalid and will be skipped. Error: {1}", file, e.Message));
                    continue;
                }
                foreach (WzObject obj in objs)
                {
                    AddObj(parent, obj);
                    //Console.WriteLine($"Added object {obj.Name} for file '{file}'");
                    objects++;
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
    }
}
