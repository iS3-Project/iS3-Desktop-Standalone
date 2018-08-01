using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace iS3.Config
{
    // Digital object type helper:
    //      return all available DGObject subclass names.
    //
    public class ObjectTypeHelper
    {
        static List<Assembly> _loadedExtensions = new List<Assembly>();
        static List<string> DObjectTypes = null;
      
        // return all available DGObject subclass names as a list of string
        //
        public static List<string> GetDObjectTypes()
        {
            if (DObjectTypes == null)
            {
                DObjectTypes = new List<string>();
                loadFromExtensions(DObjectTypes);
                DObjectTypes.Sort();
            }
            return DObjectTypes;
        }

        // Summary:
        //     Load extensions which are located in the bin\extensions\ directory.
        static void loadFromExtensions(List<string> DObjectTypes)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = exeAssembly.Location;
            string exePath = Path.GetDirectoryName(exeLocation);
            string extensionsPath = exePath + "\\extensions";

            if (!Directory.Exists(extensionsPath))
                return;

            // try to load *.dll in bin\extensions\
            var files = Directory.EnumerateFiles(extensionsPath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                // skip the assembly that has been loaded
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedExtensions.Add(assembly);
            }

            // call init() in extensions
            foreach (Assembly assembly in _loadedExtensions)
            {
                // call init() function in the loaded assembly
                var types = from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(IS3.Core.DGObject))
                            select type;
                foreach (var type in types)
                {
                    DObjectTypes.Add(type.Name);
                }
            }
        }
    }
}
