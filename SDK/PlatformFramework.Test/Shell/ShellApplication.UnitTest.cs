using System;
using System.Windows.Controls;
using Calvor.SDK.PlatformFramework.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calvor.SDK.PlatformFramework;
using System.ComponentModel.Composition;
using System.Windows;
using System.ComponentModel.Composition.Hosting;

namespace Calvor.SDK.PlatformFrameworkTest
{
    [TestClass]
    public class ShellAppliationUnitTest
    {
        [TestMethod]
        public void ShellApplication_RunShellApplication_Successfully()
        {
            ShellApplication.CreateAndRunShellApplication();
        }
        
        [TestMethod]
        public void ShellApplication_ImportExports_Successfully()
        {
            var shellApplication = ShellApplication.CreateAndRunShellApplication();
            Assert.IsNotNull(shellApplication.Host.GetSharedExportedValue<IMockInterface>());
        }

        public interface IMockInterface
        { }

        [Export(typeof(IMockInterface))]
        public class MockExport : IMockInterface
        { }
    }
}
