using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calvor.Core.Composition;

namespace Calvor.SDK.PlatformFramework.Shell
{
    /// <summary>
    /// StudioWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StudioWindow : Window
    {
        public StudioWindow(ICompositionHost host)
        {
            Host = host;
            InitializeComponent();
        }

        public ICompositionHost Host { get; private set; }

        [ImportMany(typeof(IMainWindow))]
        public IEnumerable<Lazy<IMainWindow, IMainViewSiteMetadata>> SidePanelsProviders { get; private set; }

        private void StudioWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Host.SatisfyImportsOnce(this);
            foreach (var sidePanelProvider in SidePanelsProviders)
            {
                _mainViewSite.Items.Add(new TabItem
                {
                    Content = sidePanelProvider.Value.GetMainWindow(),
                    Header = sidePanelProvider.Metadata.Name
                });
            }
            _mainViewSite.SelectedIndex = 0;
            _sideView.Children.Add(SidePanelsProviders.First().Value.CreateSidePanel());
        }
    }
}
