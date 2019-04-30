using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    // Summary:
    //     ExtensionManager:
    //     (1) create object from class name
    //     (2) object data to string
    //     (3) collection data to string
    //
    public class ExtensionManager
    {
        static IMainFrame _mainFrame;
        static ExtensionManager extensionManager;
        public static ExtensionManager GetInstance(IMainFrame mainFrame = null)
        {
            if (extensionManager == null)
            {
                extensionManager = new ExtensionManager(mainFrame);
            }
            return extensionManager;
        }
        private ExtensionManager(IMainFrame mainFrame)
        {
            _mainFrame = mainFrame == null ? _mainFrame : mainFrame;
        }

        public static Dictionary<string, Extensions> domainExtension = new Dictionary<string, Extensions>();
        public static Dictionary<string, Assembly> assemblyDict = new Dictionary<string, Assembly>();
        List<Assembly> _loadedExtensions = new List<Assembly>();
        public void LoadExtension()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = System.IO.Directory.GetCurrentDirectory();
            string extensionsPath = exeLocation + "\\extensions";

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
                            where type.IsSubclassOf(typeof(Extensions))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    Extensions extension = obj as Extensions;
                    if (extension == null)
                        continue;
                    string msg = extension.init();
                    _mainFrame.output(msg);

                }
                //save assembly by the dll name
                //
                if (assembly.FullName.Split(',').Length > 0)
                {
                    assemblyDict[assembly.FullName.Split(',')[0]] = assembly;
                }
            }
        }

        static List<Assembly> _loadedToolboxes = new List<Assembly>();
        // Summary:
        //     Load tools which are located in the bin\tools\ directory.
        public Tuple<List<ToolTreeItem>, List<iS3MenuItem>> loadToolboxes()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = System.IO.Directory.GetCurrentDirectory();
            string toolsPath = exeLocation + "\\tools";

            if (!Directory.Exists(toolsPath))
                return null;

            // try to load *.dll in bin\tools\
            var files = Directory.EnumerateFiles(toolsPath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedToolboxes.Add(assembly);
            }
            List<ToolTreeItem> toolTreeItems = new List<ToolTreeItem>();
            List<iS3MenuItem> menuTreeItems = new List<iS3MenuItem>();
            foreach (Assembly assembly in _loadedToolboxes)
            {
                //loadUI(assembly);
                // call init() function in the loaded assembly
                var types = from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(iS3.Core.Extensions))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    iS3.Core.Extensions extension = obj as iS3.Core.Extensions;
                    if (extension == null)
                        continue;
                    string msg = extension.init();
                    _mainFrame.output(msg);
                }

                // call tools.treeItems() can add it to ToolsPanel
                types = from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(Tools))
                        select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    Tools tools = obj as Tools;
                    List<ToolTreeItem> treeitems = tools.treeItems().ToList();
                    List<iS3MenuItem> menuitems = tools.menuItems().ToList();
                    if (treeitems == null)
                        continue;
                    toolTreeItems.AddRange(treeitems);
                    menuTreeItems.AddRange(menuitems);
                }
            }
            return new Tuple<List<ToolTreeItem>, List<iS3MenuItem>>(toolTreeItems, menuTreeItems);
        }
        static List<Assembly> _loadedExteralControls = new List<Assembly>();
        // Summary:
        //     Load tools which are located in the bin\tools\ directory.
        public List<ControlTreeItem> loadedExteralControls()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Assembly exeAssembly = Assembly.GetExecutingAssembly();
            string exeLocation = System.IO.Directory.GetCurrentDirectory();
            //string exeLocation = exeAssembly.Location;
            string controlsPath = exeLocation + "\\controls";

            if (!Directory.Exists(controlsPath))
                return null;

            // try to load *.dll in bin\tools\
            var files = Directory.EnumerateFiles(controlsPath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedExteralControls.Add(assembly);
            }
            List<ControlTreeItem> controlTreeItems = new List<ControlTreeItem>();
            foreach (Assembly assembly in _loadedExteralControls)
            {
                //loadUI(assembly);
                // call init() function in the loaded assembly
                var types = from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(iS3.Core.Extensions))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    iS3.Core.Extensions extension = obj as iS3.Core.Extensions;
                    if (extension == null)
                        continue;
                    string msg = extension.init();
                    _mainFrame.output(msg);
                }

                // call tools.treeItems() can add it to ToolsPanel
                types = from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(ExteralControls))
                        select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    ExteralControls controls = obj as ExteralControls;
                    List<ControlTreeItem> treeitems = controls.treeItems().ToList();
                    if (treeitems == null)
                        continue;
                    controlTreeItems.AddRange(treeitems);
                }
            }
            return controlTreeItems;
        }
    }
}
