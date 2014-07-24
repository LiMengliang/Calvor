using System;
using System.Windows;

namespace Calvor.SDK.PlatformFramework.Shell
{
    public interface IMainViewSite
    {
        int CreateAndShowSidePanel(Lazy<ISidePanelType> sidePanel);
    }
}
