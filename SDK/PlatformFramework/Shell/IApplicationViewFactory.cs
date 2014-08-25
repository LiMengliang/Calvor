using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calvor.SDK.PlatformFramework.Shell
{
    public interface IApplicationViewFactory
    {
        UIElement CreateSideView();
        UIElement CreateMainView();
    }
}
