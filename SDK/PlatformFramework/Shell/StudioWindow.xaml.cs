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
        
        [ImportMany(typeof(IApplicationViewFactory))]
        public IEnumerable<Lazy<IApplicationViewFactory>> ApplicationViewFactory { get; private set; }

        private void StudioWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Host.SatisfyImportsOnce(this);
            _sideViewSite.Children.Add(ApplicationViewFactory.First().Value.CreateSideView());
            _mainViewSite.Children.Add(ApplicationViewFactory.First().Value.CreateMainView());
        }
    }
}
