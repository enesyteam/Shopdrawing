using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace TestApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static CompositionContainer compositionContainer;

        public static CompositionContainer CompositionContainer
        {
            get { return compositionContainer; }
        }
        internal static IList<ExceptionData> StartupExceptions = new List<ExceptionData>();
        internal class ExceptionData
        {
            public Exception Exception;
            public string PluginName;
        }

        public App()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
            // Don't use DirectoryCatalog, that causes problems if the plugins are from the Internet zone
            // see http://stackoverflow.com/questions/8063841/mef-loading-plugins-from-a-network-shared-folder
            string appPath = Path.GetDirectoryName(typeof(App).Module.FullyQualifiedName);
            foreach (string plugin in Directory.GetFiles(appPath, "*.Plugin.dll"))
            {
                string shortName = Path.GetFileNameWithoutExtension(plugin);
                try
                {
                    var asm = Assembly.Load(shortName);
                    asm.GetTypes();
                    catalog.Catalogs.Add(new AssemblyCatalog(asm));
                }
                catch (Exception ex)
                {
                    // Cannot show MessageBox here, because WPF would crash with a XamlParseException
                    // Remember and show exceptions in text output, once MainWindow is properly initialized
                    StartupExceptions.Add(new ExceptionData { Exception = ex, PluginName = shortName });
                }
            }

            compositionContainer = new CompositionContainer(catalog);
        }


    }
}
