//-----------------------------------------------------------------------
// <copyright file="CompositionExtension.WPF.cx" company="National Instruments">
//     Copyright (c) National Instruments.  All rights reserved.
// </copyright>
// Design document is located here:
// http:http://nicentral.natinst.com/confluence/display/Blueprint/NextGen+Plug-in+System
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using Calvor.Core;
using Calvor.Core.Composition;

namespace NationalInstruments.Composition
{
    /// <summary>
    /// Provides the standard implementation of the ICompositionHost interface for this next gen system.
    /// This implementation is suitable for use in your application as well as for unit tests.
    /// Refer to the documentation on the next-gen plug-in system for more information on the
    /// role of the composition host.
    /// </summary>
    public sealed class CompositionHost : ICompositionHost
    {
        #region Fields

        private bool _disposed;
        private CompositionContainer _container;
        private CompositionContainer _containerDisposeAtTheEnd;
        private readonly HashSet<Assembly> _composedAssemblies;
        private CompositionSet _compositionSet;
        private AggregateCatalog _catalog;

        #endregion

        #region Constructor/Initialization

        /// <summary>
        /// Initializes a new instance of the CompositionHost class.
        /// </summary>
        public CompositionHost()
        {
            _composedAssemblies = new HashSet<Assembly>();
        }

        /// <summary>
        /// Initializes the CompositionHost after construction
        /// </summary>
        /// <param name="set">CompositionSet this CompositionHost uses</param>
        /// <param name="container">CompositionContainer this CompositionHost uses</param>
        /// <param name="containerDisposeAtTheEnd">CompositionContainer that will get disposed when the CompositionHost is disposed</param>
        /// <param name="catalog">Catalog this CompositionHost uses</param>
        internal void Initialize(CompositionSet set, CompositionContainer container, CompositionContainer containerDisposeAtTheEnd, AggregateCatalog catalog)
        {
            _compositionSet = set;
            _container = container;
            _containerDisposeAtTheEnd = containerDisposeAtTheEnd;
            _catalog = catalog;

            var compositionHostManager = _container.GetExportedValue<ICompositionHostManager>();
            ((CompositionHostManager)compositionHostManager).AddCompositionHost(this);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Initializes a new composition host with the components provided in the specified component catalog
        /// </summary>
        /// <param name="catalog">The catalog to use for composition</param>
        /// <returns>The newly created composition host</returns>
        public static ICompositionHost InitializeWithCatalog(ComposablePartCatalog catalog)
        {
            return InitializeWithCatalog(catalog, null, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Initializes a new composition host with the components provided in the specified component catalog
        /// </summary>
        /// <param name="catalog">The catalog to use for composition</param>
        /// <param name="filter">Optional filter which is called to filter the catalog on a per part basis</param>
        /// <returns>The newly created composition host</returns>
        public static ICompositionHost InitializeWithCatalog(ComposablePartCatalog catalog, ICompositionFilter filter)
        {
            return InitializeWithCatalog(catalog, filter, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Initializes a new composition host with the components provided in the specified component catalog
        /// </summary>
        /// <param name="catalog">The catalog to use for composition of the root and leaf composition containers</param>
        /// <param name="filter">Filter for eliminating exports from the catalog</param>
        /// <param name="componentDirectory">The directory to use as the root of composition</param>
        /// <returns>The newly created composition host</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "CompositionSet is cached in a static list")]
        public static ICompositionHost InitializeWithCatalog(ComposablePartCatalog catalog, ICompositionFilter filter, string componentDirectory)
        {
            var set = CompositionSet.FindCompositionSet(catalog);
            if (set == null)
            {
                set = new CompositionSet(catalog, componentDirectory);
            }

            CompositionHost host = set.CreateHost(null, filter);
            return host;
        }

        /// <summary>
        /// Initializes a new composition host using the standard component directory
        /// </summary>
        /// <param name="filter">Optional filter which is called to filter the catalog on a per part basis</param>
        /// <returns>The newly created composition host</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Not Appropriate")]
        public static ICompositionHost InitializeNew(ICompositionFilter filter)
        {
            return InitializeNew(filter, null);
        }

        /// <summary>
        /// Initializes a new composition host loading the components from the provided component directory
        /// </summary>
        /// <param name="componentPath">The full path to the directory to look in for composition assemblies</param>
        /// <returns>The newly created composition host</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Not Appropriate")]
        public static ICompositionHost InitializeNew(string componentPath)
        {
            return InitializeNew(null, componentPath);
        }

        /// <summary>
        /// Initializes a new composition host loading the components from the provided component directory
        /// </summary>
        /// <param name="filter">Optional filter which is called to filter the catalog on a per part basis</param>
        /// <param name="componentPath">The full path to the directory to look in for composition assemblies</param>
        /// <returns>The newly created composition host</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Not Appropriate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The item is assigned to a value in another object which is responsible for desposing it")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile", Justification = "We need to call Assembly.LoadFile to load our assemblies.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any Cache Failure Exception, there are lots, and we should be able to recover from any of them")]
        public static ICompositionHost InitializeNew(ICompositionFilter filter, string componentPath)
        {
            if (string.IsNullOrEmpty(componentPath))
            {
#if !MONOTOUCH
                componentPath = typeof(CompositionHost).Assembly.Location;
                componentPath = Path.GetDirectoryName(componentPath);
                ////componentPath = componentPath.Substring(0, componentPath.LastIndexOf('\\'));
#else
                // Xamarin iOS bundles for the simulator use shortcuts to dlls that live in the general Assemblies output
                // directory, instead of pointing to the copies in each app's output directory or having real copies of the dlls
				// in the bundle. This makes using assembly location problematic because the assembly comes from the general
				// Assemblies directory, so we'll end up trying to use a componentPath that has all dlls from Everything.MonoTouch.sln.
				// If we use the bundle path instead, everything works out as we want/expect.
                componentPath = MonoTouch.Foundation.NSBundle.MainBundle.BundlePath;
#endif
            }

            CompositionHost host = null;
            using (var compositionSetLock = CompositionSet.InitCompositionSet(filter, componentPath))
            {
                var set = CompositionSet.FindCompositionSet(componentPath);
                if (set != null)
                {
                    host = set.CreateHost(null, filter);
                }
            }

            // Setup the container and create the host
#if _DEBUG
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
#endif
            ////Log.WriteLine("******* Composition for {0} took {1:0.00} ms", componentPath, (end - start).TotalMilliseconds);
            return host;
        }

        /// <summary>
        /// Initializes a new composition host using the standard component directory
        /// </summary>
        /// <returns>The newly created composition host</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Not Appropriate")]
        public static ICompositionHost InitializeNew()
        {
            return InitializeNew(null, null);
        }

        #endregion

        #region ICompositionHost Implementation

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Lameness")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "BugInCodeAnalysisRule")]
        T ICompositionHost.CreateInstance<T>()
        {
            DoDisposedCheck();
            var export = GetNonSharedExportedValueOrDefault<T>(_container);
            if (export != null)
            {
                ValidateNonShared(export.GetType());
                return export;
            }

            T result = new T();
            var selfComposes = result as IUseComposition;
            if (selfComposes != null)
            {
                selfComposes.Host = this;
                var needsSatisfied = result as IPartImportsSatisfiedNotification;
                if (needsSatisfied != null)
                {
                    needsSatisfied.OnImportsSatisfied();
                }
            }
            else
            {
                _container.SatisfyImportsOnce(result);
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method wraps a method that also requires a type parameter.")]
        T ICompositionHost.GetSharedExportedValue<T>()
        {
            DoDisposedCheck();
            return _exportCache.GetExportedValue<T>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method wraps a method that also requires a type parameter.")]
        T ICompositionHost.GetNonSharedExportedValue<T>()
        {
            DoDisposedCheck();
            var result = _container.GetExportedValue<T>();
            ValidateNonShared(result.GetType());
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method wraps a method that also requires a type parameter.")]
        T ICompositionHost.GetSharedExportedValueOrDefault<T>()
        {
            DoDisposedCheck();
            T export;
            _exportCache.TryGetExportedValue(out export);
            return export;
        }

        T ICompositionHost.GetExportedValue<T>(string contractName)
        {
            DoDisposedCheck();
            return _container.GetExportedValue<T>(contractName);
        }

        IEnumerable<Lazy<T, TMetadata>> ICompositionHost.GetExports<T, TMetadata>()
        {
            DoDisposedCheck();
            return _container.GetExports<T, TMetadata>();
        }

        IEnumerable<Lazy<T>> ICompositionHost.GetExports<T>()
        {
            DoDisposedCheck();
            return _container.GetExports<T>();
        }
        
        IEnumerable<Lazy<object, object>> ICompositionHost.GetExports(Type type, Type metadataViewType, string contractName)
        {
            DoDisposedCheck();
            return _container.GetExports(type, metadataViewType, contractName);
        }

        IEnumerable<Lazy<T, TMetadataView>> ICompositionHost.GetExports<T, TMetadataView>(string contractName)
        {
            DoDisposedCheck();
            return _container.GetExports<T, TMetadataView>(contractName);
        }

        IEnumerable<Export> ICompositionHost.GetExports(ImportDefinition definition)
        {
            DoDisposedCheck();
            return _container.GetExports(definition);
        }
        
        IEnumerable<T> ICompositionHost.GetSharedExportedValues<T>()
        {
            DoDisposedCheck();
            return _exportCache.GetExportedValues<T>();
        }

        IEnumerable<T> ICompositionHost.GetNonSharedExportedValues<T>()
        {
            DoDisposedCheck();
            var result = _container.GetExportedValues<T>();
            if (GlobalTestingState.IsDebugOrTestCode)
            {
                ValidateAtLeastOneNonShared((IEnumerable<object>)result, typeof(T));
            }
            return result;
        }

        void ICompositionHost.Compose(CompositionBatch batch)
        {
            DoDisposedCheck();
            _container.Compose(batch);
        }

        ComposablePart ICompositionHost.SatisfyImportsOnce(object attributedPart)
        {
            DoDisposedCheck();
            return _container.SatisfyImportsOnce(attributedPart);
        }

        void ICompositionHost.ComposeParts(params object[] attributedParts)
        {
            DoDisposedCheck();
            _container.ComposeParts(attributedParts);
        }

        void ICompositionHost.AddToComposition(Assembly assembly)
        {
            DoDisposedCheck();
            lock (_catalog)
            {
                if (!_composedAssemblies.Contains(assembly) && !_compositionSet.Assemblies.Contains(assembly))
                {
                    var participates = PlatformSupport.PlatformGetCustomAttribute(assembly, typeof(ParticipatesInCompositionAttribute));
                    if (participates != null)
                    {
                        _composedAssemblies.Add(assembly);
                        _catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                    }
                }
            }
        }

        void ICompositionHost.AddToComposition(ComposablePartCatalog composablePartCatalog)
        {
            DoDisposedCheck();
            lock (_catalog)
            {
                _catalog.Catalogs.Add(composablePartCatalog);
            }
        }
        
        #endregion

        #region Internal Members

        /// <summary>
        /// Used for testing purposes
        /// </summary>
        internal int ComposablePartCount
        {
            get { return _container.Catalog.Parts.Count(); }
        }

        /// <summary>
        /// Wrapper around CompositionContainer.GetSharedExportedValueOrDefault which catches exceptions
        /// </summary>
        /// <typeparam name="T">The type of object to get</typeparam>
        /// <param name="container">The composition container</param>
        /// <returns>The exported value or default</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Thismethodwrapsamethodthatalsorequiresatypeparameter.")]
        private static T GetNonSharedExportedValueOrDefault<T>(CompositionContainer container) where T : class
        {
            try
            {
                T t = container.GetExportedValueOrDefault<T>();
                return t;
            }
            catch (CompositionException e)
            {
                return default(T);
            }
#if MONO
            catch (Exception e)
            {
                Log.WriteLine("Composition Exception: " + e.Message);
                return default(T);
            }
#endif
        }

        /// <summary>
        /// Checks that an export gotten from the composition container is a non shared instance.
        /// </summary>
        /// <param name="type">The type being exported</param>
        private static void ValidateNonShared(Type type)
        {
            if (!GlobalTestingState.IsDebugOrTestCode)
            {
                return;
            }

            if (_creationPolicyMemo[type] != CreationPolicy.NonShared)
            {
                throw new InvalidOperationException("MEF exported type: " + type.FullName + " is used by CreateInstance or GetNonSharedExportedValue, but it is not marked as non-shared.\n" +
                    "You need to add the \n\n[PartCreationPolicy(CreationPolicy.NonShared)]\n\nAttribute to the type definition or use GetSharedExportedValue instead.");
            }
        }

        /// <summary>
        /// Checks that an enumerable of exports gotten from the composition container has at least one non shared instance.
        /// </summary>
        /// <param name="exportedObjects">The objects being exported</param>
        /// <param name="baseType">The base type requested for the export</param>
        private static void ValidateAtLeastOneNonShared(IEnumerable<object> exportedObjects, Type baseType)
        {
            if (!GlobalTestingState.IsDebugOrTestCode || !exportedObjects.Any())
            {
                return;
            }

            if (exportedObjects.Any(export => _creationPolicyMemo[export.GetType()] == CreationPolicy.NonShared))
            {
                return;
            }

            throw new InvalidOperationException("MEF exported types for: " + baseType.FullName + " is used by GetNonSharedExportedValues, but none of the found types are marked as non-shared.\n" +
                "You need to add the \n\n[PartCreationPolicy(CreationPolicy.NonShared)]\n\nAttribute to the type definition or use GetSharedExportedValues instead.");
        }

        /// <summary>
        /// Checks that an export gotten from the composition container is a shared instance.
        /// </summary>
        /// <param name="type">The type being exported</param>
        private static void ValidateShared(Type type)
        {
            if (!GlobalTestingState.IsDebugOrTestCode)
            {
                return;
            }

            CreationPolicy policy = _creationPolicyMemo[type];
            if (policy != CreationPolicy.Shared && policy != CreationPolicy.Any)
            {
                throw new InvalidOperationException("MEF exported type: " + type.FullName + " is used by GetSharedExportedValue or GetSharedExportedValueOrDefault, but it is not marked as shared.\n" +
                    "You need to add the \n\n[PartCreationPolicy(CreationPolicy.Shared)]\n\nAttribute to the type definition or use CreateInstance or GetNonSharedExportedValue instead.");
            }
        }

        /// <summary>
        /// Checks that an enumerable of exports gotten from the composition container are all shared instances.
        /// </summary>
        /// <param name="exportedObjects">The objects being exported</param>
        private static void ValidateAllShared(IEnumerable<object> exportedObjects)
        {
            if (!GlobalTestingState.IsDebugOrTestCode)
            {
                return;
            }

            foreach (var export in exportedObjects)
            {
                CreationPolicy policy = _creationPolicyMemo[export.GetType()];
                if (policy != CreationPolicy.Shared && policy != CreationPolicy.Any)
                {
                    throw new InvalidOperationException("MEF exported type: " + export.GetType().FullName + " is used by GetSharedExportedValues but it is not marked as shared.\n" +
                        "You need to add the \n\n[PartCreationPolicy(CreationPolicy.Shared)]\n\nAttribute to the type definition or use GetNonSharedExportedValues instead.");
                }
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Called to Dispose the composition host
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                var compositionHostManager = _container.GetExportedValue<ICompositionHostManager>();
                ((CompositionHostManager)compositionHostManager).RemoveCompositionHost(this);

                _compositionSet.RemoveFromSet(_container);
                _container.Dispose();
                _exportCache = null;
                if (_containerDisposeAtTheEnd != null)
                {
                    _containerDisposeAtTheEnd.Dispose();
                }
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
            }
        }

        private void DoDisposedCheck()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("CompositionHost");
            }
        }

        #endregion

        private readonly List<IDisposable> _disposables = new List<IDisposable>(); 
        /// <summary>
        /// Add additional items to be cleaned up on dispose
        /// </summary>
        /// <param name="disposable">a disposable</param>
        internal void AddToCleanup(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
}
