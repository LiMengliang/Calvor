using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calvor.Core;
using System.Windows;
using Calvor.Core.Composition;

namespace Calvor.SDK.PlatformFramework.Shell
{
    public class ShellApplication : Application
    {
        private static StudioWindow _mainWindow;

        #region Properties

        public ICompositionHost Host { get; private set; }

        #endregion

        #region Constructors

        public ShellApplication()
        {
            
        }

        #endregion

        public static ShellApplication CreateAndRunShellApplication()
        {
            var shellApplication = new ShellApplication();
            shellApplication.Host = new CompositionHost();
            ((CompositionHost)(shellApplication.Host)).InitializeWithCatalog(new DirectoryCatalog(new DirectoryInfo(typeof(ShellApplication).Assembly.Location).Parent.FullName));
            shellApplication.Host.SatisfyImportsOnce(shellApplication);
            _mainWindow = new StudioWindow(shellApplication.Host);
            _mainWindow.Show();
            shellApplication.Run(_mainWindow);
            return shellApplication;
        }
    }
}
