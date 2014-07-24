using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calvor.SDK.PlatformFramework.Shell;

namespace SimpleWindowApplication
{
    [Export]
    sealed class SimpleWindow : ShellApplication
    {
        [STAThread]
        static void Main(string[] args)
        {
            CreateAndRunShellApplication();
        }
    }
}
