using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        private void StudioWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // foreach (var sidePanel in _sidePanels)
            // {
            //     var button = new Button();
            //     _sideViewHead.Children.Add(button);
            // }
            var sidePanelCreators = Host.GetSharedExportedValues<ISidePanelType>();
            foreach (var sidePanelCreator in sidePanelCreators)
            {
                _sideViewBody.Children.Add(sidePanelCreator.CreateSidePanel());
            }
            // _sideViewBody.Children.Add(_sidePanels.Value.CreateSidePanel());
        }
    }
}
