using System.Windows;

namespace Calvor.SDK.PlatformFramework.Shell
{
    public interface IMainWindow
    {
        UIElement CreateSidePanel();
        UIElement GetMainWindow();
    }
}
