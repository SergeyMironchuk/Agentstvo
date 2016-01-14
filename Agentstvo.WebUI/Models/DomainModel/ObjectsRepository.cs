using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Agentstvo.WebUI.Models.DomainModel
{
    public static class ObjectsRepository
    {
        private static readonly object LockObject = new object();
        private static string _version = "";

        private static IList<ObjectForSale> _objectsForSale;
        public static readonly string CatalogFolder = HttpContext.Current.Server.MapPath("~/App_Data/Catalog/");

        public static void CheckVersion()
        {
            var files = Directory.GetFiles(CatalogFolder, "*.version");
            if (files.Length > 0)
            {
                var version = files.Select(f => Path.GetFileNameWithoutExtension(f) ?? "0").OrderByDescending(f => f).First();
                if (version != _version)
                {
                    _version = version;
                    _objectsForSale = null;
                }
            }
        }

        public static IList<ObjectForSale> ObjectForSales
        {
            get
            {
                _objectsForSale = _objectsForSale ?? FillCatalog();
                return _objectsForSale;

            }
        }

        public static IList<ObjectForSale> FillCatalog()
        {
            lock (LockObject)
            {
                var result = new List<ObjectForSale>();
                var catalogFolders = Directory.GetDirectories(CatalogFolder, "*", SearchOption.TopDirectoryOnly);
                int id = 0;
                foreach (var folder in catalogFolders)
                {
                    var catalogFiles = Directory.GetFiles(folder, "*");
                    var mainFile = catalogFiles.FirstOrDefault(IsMainGraphicsFile);
                    if (mainFile != null)
                    {
                        //id++;
                        var objectForSale = new ObjectForSale
                        {
                            Id = id++,
                            ImageLocation = Path.GetFileName(folder) + @"___" + Path.GetFileName(mainFile),
                            ImagesLocation = new HashSet<ObjectForSale>(catalogFiles.Where(IsGraphicsFile)
                                .Select(f => new ObjectForSale
                                {
                                    ImageLocation = Path.GetFileName(folder) + @"___" + Path.GetFileName(f),
                                    CurrentClass = ""
                                })
                                .ToArray())
                        };
                        objectForSale.Description = objectForSale.SetDescription();
                        result.Add(objectForSale);
                    }
                }
                return result;
            }
        }

        private static bool IsMainGraphicsFile(string f)
        {
            return string.Equals(Path.GetFileName(f),"main.jpg",StringComparison.OrdinalIgnoreCase) 
                   || string.Equals(Path.GetFileName(f), "main.png", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGraphicsFile(string f)
        {
            return string.Equals(Path.GetExtension(f),".jpg",StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(f), ".png", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetObjectDescription(int id)
        {
            var objectForSale = ObjectForSales.FirstOrDefault(o => o.Id == id);
            var result = SetDescription(objectForSale);
            return result;
        }

        private static string SetDescription(this ObjectForSale objectForSale)
        {
            string result = "";
            if (objectForSale != null)
            {
                if (string.IsNullOrEmpty(objectForSale.Description))
                {
                    var imageLocation = CatalogFolder + objectForSale.ImageLocation.Replace("___", "/");
                    var objectPath = Path.GetDirectoryName(imageLocation);
                    var mainFile = Path.GetFileNameWithoutExtension(imageLocation);
                    var descriptionFile = objectPath + @"\" + mainFile + ".txt";
                    if (File.Exists(descriptionFile))
                    {
                        objectForSale.Description = File.ReadAllText(descriptionFile);
                    }
                }
                result = objectForSale.Description;
            }
            return result;
        }
    }
}