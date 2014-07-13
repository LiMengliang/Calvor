using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Calvor.Core.Composition
{
    public class CompositionSet : IDisposable
    {
        private static readonly List<CompositionSet> _compositionSets = new List<CompositionSet>();
        private CompositionContainer _rootContainer;
        private readonly ComposablePartCatalog _catalog;
        private readonly List<CompositionContainer> _activeLeftContainers = new List<CompositionContainer>();
        private int _lockCount;
        internal string ComponentPath
        {
            get;
            private set;
        }

        #region Constructors

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NationalInstruments.LabVIEW.Tools.StyleCopRules.NIDocumentationRule", "NI1003:DocumentationRequired", Justification = "TODO DOC")]
        internal CompositionSet(ComposablePartCatalog catalog, string componentPath) :
            this(catalog)
        {
            ComponentPath = componentPath;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is stored in a local")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("NationalInstruments.LabVIEW.Tools.StyleCopRules.NIDocumentationRule", "NI1003:DocumentationRequired", Justification = "TODO DOC")]
        internal CompositionSet(ComposablePartCatalog catalog)
        {
            lock (_compositionSets)
            {
                _compositionSets.Add(this);
            }
            _catalog = catalog;
            if (_catalog != null)
            {
                _rootContainer = new CompositionContainer(_catalog, true);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a CompositionSet based on the assemblies found in the specified component path directory.
        /// The lifetime of the composition set is guaranteed to be at least as long as the Disposable object returned.
        /// When the composition set is no longer needed the returned disposable should be Disposed.
        /// </summary>
        /// <remarks>
        /// If this method is called more than once with the same component path, the same ComponentSet is initialized
        /// and there are essentially 2 locks of the same set.  The component path will not be rechecked for new assemblies.
        /// </remarks>
        /// <param name="filter">Optional filter which is called to filter the catalog on a per part basis</param>
        /// <param name="componentPath">The component path to initialize from</param>
        /// <returns>Lock object that should be Disposed when the CompositionSet is no longer needed.</returns>
        public static IDisposable InitCompositionSet(ICompositionFilter filter, string componentPath)
        {
            CompositionSet set = FindCompositionSet(componentPath);
            if (set == null)
            {
                var aggregateCatalog = new AggregateCatalog();

                // Load or create the catalog
                foreach (var file in Directory.GetFiles(componentPath, "*.dll"))
                {
                    try
                    {
                        AssemblyName assemblyName = GetNameForManagedAssemblies(file);
                        bool tryLoad = assemblyName != null && (
                                               (assemblyName.ProcessorArchitecture == ProcessorArchitecture.MSIL)
                                            || (assemblyName.ProcessorArchitecture == ProcessorArchitecture.X86 && IntPtr.Size == 4)
                                            || (assemblyName.ProcessorArchitecture == ProcessorArchitecture.Amd64 && IntPtr.Size == 8));

                        if (tryLoad)
                        {
                            // only attempt to load a AnyCpu assembly or assemblies built for the cpu we're running on
                            ComposablePartCatalog catalog = LoadAssemblyCatalog(file);

                            if (catalog != null)
                            {
                                aggregateCatalog.Catalogs.Add(catalog);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                set = new CompositionSet(aggregateCatalog, componentPath);
            }

            return new CompositionSetLock(set);
        }

        /// <summary>
        /// Load the assembly catalog from cache if possible, creating a cache file if it can.
        /// </summary>
        /// <param name="assemblyFile">Assembly file name</param>
        /// <returns>MEF catalog for the assembly</returns>
        public static ComposablePartCatalog LoadAssemblyCatalog(string assemblyFile)
        {
            ComposablePartCatalog catalog = null;
            try
            {
                var assembly = Assembly.LoadFile(assemblyFile);
                catalog = new AssemblyCatalog(assembly);
            }
            catch (Exception)
            {
                if (catalog != null)
                {
                    catalog.Dispose();
                    catalog = null;
                }
            }
            return catalog;
        }

        internal static CompositionSet FindCompositionSet(string componentPath)
        {
            lock (_compositionSets)
            {
                return _compositionSets.FirstOrDefault(set => set.ComponentPath == componentPath);
            }
        }

        private static bool IsCacheFile(string filePath)
        {
            return filePath.EndsWith(".cache.dll", StringComparison.OrdinalIgnoreCase);
        }

        private static AssemblyName GetNameForManagedAssemblies(string file)
        {
            // DebuggerStepThrough so that normal, non-managed DLLs that will make this function throw will not show up in first-chance debugging.
            try
            {
                return AssemblyName.GetAssemblyName(file);
            }
            catch (BadImageFormatException)
            {
                // This is not a managed DLL.
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NationalInstruments.LabVIEW.Tools.StyleCopRules.NIDocumentationRule", "NI1003:DocumentationRequired", Justification = "TODO DOC")]
        internal void Lock()
        {
            lock (_activeLeftContainers)
            {
                _lockCount += 1;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NationalInstruments.LabVIEW.Tools.StyleCopRules.NIDocumentationRule", "NI1003:DocumentationRequired", Justification = "TODO DOC")]
        internal void Unlock()
        {
            lock (_activeLeftContainers)
            {
                _lockCount -= 1;
                DisposeIfReady();
            }
        }

        private void DisposeIfReady()
        {
            if (_activeLeftContainers.Count == 0 && _lockCount == 0)
            {
                Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (_compositionSets)
            {
                if (_compositionSets.Contains(this))
                {
                    _compositionSets.Remove(this);
                }
            }
            if (_rootContainer != null)
            {
                _rootContainer.Dispose();
                _rootContainer = null;
            }
        }
    }
}
