using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Calvor.Core.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calvor.Core.CompositionTest
{
    [TestClass]
    public class CompositionHostUnitTest
    {
        private ICompositionHost _host;

        // If you want to use MetaData, you should make Lazy import.
        [Import(typeof(IMockInterface))]
        public Lazy<IMockInterface, IMetadata> MockInterfaceImplementation { get; set; }

        [ImportMany(typeof(IMockMultiImplementInterface))]
        public IEnumerable<Lazy<IMockMultiImplementInterface>> mockMultiImpelentations { get; set; }
        
        [TestInitialize]
        public void InitializeHost()
        {
            _host = new CompositionHost();
            var compositionHost = _host as CompositionHost;
            if (compositionHost != null)
            {
                compositionHost.InitializeWithCatalog(new DirectoryCatalog(new DirectoryInfo(GetType().Assembly.Location).Parent.FullName));
            }
        }

        [TestMethod]
        public void CompositionHost_GetSharedExportedValue_Successfully()
        {
            var mockInterfaceImplementation = _host.GetSharedExportedValue<IMockInterface>();
            Assert.IsNotNull(mockInterfaceImplementation);
        }

        [TestMethod]
        public void CompositionHost_ImportOneExport_Successfully()
        {
            var compositionHost = _host as CompositionHost;
            if (compositionHost != null)
            {
                compositionHost.SatisfyImportsOnce(this);
            }
            Assert.IsNotNull(MockInterfaceImplementation);
        }

        [TestMethod]
        public void CompositionHost_GetSharedExportedValues_Successfully()
        {
            var mockMultiImplementation = _host.GetSharedExportedValues<IMockMultiImplementInterface>();
            Assert.AreEqual(2, mockMultiImplementation.Count());
        }

        [TestMethod]
        public void CompositionHost_ImportAllExports_Successfully()
        {
            var compositionHost = _host as CompositionHost;
            if (compositionHost != null)
            {
                compositionHost.SatisfyImportsOnce(this);
            }
            Assert.IsNotNull(mockMultiImpelentations);
            Assert.AreEqual(2, mockMultiImpelentations.Count());
        }

        [TestMethod]
        public void CompositionHost_GetNoneSharedExportedValue_Successfully()
        {
            var mockNoneSharedImplementation = _host.GetNonSharedExportedValue<IMockNoneSharedInterface>();
            Assert.IsNotNull(mockNoneSharedImplementation);
        }

        [TestMethod]
        public void CompositionHost_GetMetaData_Successfully()
        {
            var compositionHost = _host as CompositionHost;
            if (compositionHost != null)
            {
                compositionHost.SatisfyImportsOnce(this);
            }
            Assert.IsNotNull(MockInterfaceImplementation.Value);
            Assert.AreEqual("SimpleMetaData", MockInterfaceImplementation.Metadata.StringMetaData);
            Assert.AreEqual(1, MockInterfaceImplementation.Metadata.NumericMetaData);
        }
    }
    #region MetaData

    public interface IMetadata
    {
        // The property name should be the same with the ExportMetadata's name.
        // [ExportMetadata("StringMetaData", "SimpleMetaData")]
        // [ExportMetadata("NumericMetaData", 1)]
        int NumericMetaData { get; }
        string StringMetaData { get; }
    }

    /// <summary>
    /// Meta data definition.
    /// </summary>
    public class MyMetadata : IMetadata
    {
        private IDictionary<string, object> _metaDatas;

        public MyMetadata(IDictionary<string, object> metaDatas)
        {
            _metaDatas = metaDatas; 
        }
        public int NumericMetaData
        {
            get { return _metaDatas.ContainsKey("NumericMetaData") ? (int)_metaDatas["NumericMetaData"] : 0; }
        }

        public string StringMetaData
        {
            get { return _metaDatas.ContainsKey("StringMetaData") ? (string)_metaDatas["StringMetaData"] : string.Empty; }
        }
    }

    #endregion

    #region Plug-in definitions

    public interface IMockInterface
    {
    }

    [Export(typeof(IMockInterface))]
    [ExportMetadata("StringMetaData", "SimpleMetaData")]
    [ExportMetadata("NumericMetaData", 1)]
    public class MockInterfaceImplimentation : IMockInterface
    {
    }

    public interface IMockMultiImplementInterface
    {
    }

    [Export(typeof(IMockMultiImplementInterface))]
    public class MockMultiImpelementInterfaceA : IMockMultiImplementInterface
    {
    }

    [Export(typeof(IMockMultiImplementInterface))]
    public class MockMultiImpelementInterfaceB : IMockMultiImplementInterface
    {
    }

    public interface IMockNoneSharedInterface
    {
    }

    [Export(typeof(IMockNoneSharedInterface))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockNoneSharedInterface : IMockNoneSharedInterface
    {
    }

    #endregion
}
