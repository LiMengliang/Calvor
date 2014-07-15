using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
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
        
        private CompositionContainer _container;
        private readonly HashSet<Assembly> _composedAssemblies;
        private AggregateCatalog _catalog;

        #endregion

        #region Static Instance

        public static CompositionHost Host = new CompositionHost();

        #endregion

        #region Constructor/Initialization

        /// <summary>
        /// Initializes a new instance of the CompositionHost class.
        /// </summary>
        public CompositionHost()
        {
            _composedAssemblies = new HashSet<Assembly>();
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Initializes a new composition host with the components provided in the specified component catalog
        /// </summary>
        /// <param name="catalog">The catalog to use for composition</param>
        public void InitializeWithCatalog(ComposablePartCatalog catalog)
        {
            InitializeWithCatalog(catalog, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Initializes a new composition host with the components provided in the specified component catalog
        /// </summary>
        /// <param name="catalog">The catalog to use for composition of the root and leaf composition containers</param>
        /// <param name="componentDirectory">The directory to use as the root of composition</param>
        public void InitializeWithCatalog(ComposablePartCatalog catalog, string componentDirectory)
        {
            var directoryCatalog = new DirectoryCatalog(componentDirectory);
            _catalog = new AggregateCatalog(new Collection<ComposablePartCatalog>
            {
                catalog,
                directoryCatalog
            });
            _container = new CompositionContainer(_catalog);
        }

        #endregion

        #region ICompositionHost Implementation

        /// <inheritdoc />
        T ICompositionHost.CreateInstance<T>()
        {
            var export = GetNonSharedExportedValueOrDefault<T>(_container);
            if (export != null)
            {
                ValidateNonShared(export.GetType());
                return export;
            }
            var instance = new T();
            _container.SatisfyImportsOnce(instance);
            return instance;
        }

        /// <inheritdoc />
        T ICompositionHost.GetSharedExportedValue<T>()
        {
            var result = _container.GetExportedValue<T>();
            ValidateShared(result.GetType());
            return result;
        }

        /// <inheritdoc />
        T ICompositionHost.GetNonSharedExportedValue<T>()
        {
            var result = _container.GetExportedValue<T>();
            ValidateNonShared(result.GetType());
            return result;
        }

        /// <inheritdoc />
        T ICompositionHost.GetSharedExportedValueOrDefault<T>()
        {
            try
            {
                return _container.GetExportedValue<T>();
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <inheritdoc />
        T ICompositionHost.GetExportedValue<T>(string contractName)
        {
            return _container.GetExportedValue<T>(contractName);
        }

        /// <inheritdoc />
        IEnumerable<Lazy<T, TMetadata>> ICompositionHost.GetExports<T, TMetadata>()
        {
            return _container.GetExports<T, TMetadata>();
        }

        /// <inheritdoc />
        IEnumerable<Lazy<T>> ICompositionHost.GetExports<T>()
        {
            return _container.GetExports<T>();
        }

        /// <inheritdoc />
        IEnumerable<Lazy<object, object>> ICompositionHost.GetExports(Type type, Type metadataViewType, string contractName)
        {
            return _container.GetExports(type, metadataViewType, contractName);
        }

        /// <inheritdoc />
        IEnumerable<Lazy<T, TMetadataView>> ICompositionHost.GetExports<T, TMetadataView>(string contractName)
        {
            return _container.GetExports<T, TMetadataView>(contractName);
        }

        /// <inheritdoc />
        IEnumerable<Export> ICompositionHost.GetExports(ImportDefinition definition)
        {
            return _container.GetExports(definition);
        }

        /// <inheritdoc />
        IEnumerable<T> ICompositionHost.GetSharedExportedValues<T>()
        {
            var result = _container.GetExportedValues<T>();
            ValidateAllShared((IEnumerable<object>)result);
            return _container.GetExportedValues<T>();
        }

        /// <inheritdoc />
        IEnumerable<T> ICompositionHost.GetNonSharedExportedValues<T>()
        {
            var result = _container.GetExportedValues<T>();
            ValidateAtLeastOneNonShared((IEnumerable<object>)result, typeof(T));
            return result;
        }

        /// <inheritdoc />
        void ICompositionHost.Compose(CompositionBatch batch)
        {
            _container.Compose(batch);
        }

        /// <inheritdoc />
        ComposablePart ICompositionHost.SatisfyImportsOnce(object attributedPart)
        {
            return _container.SatisfyImportsOnce(attributedPart);
        }

        /// <inheritdoc />
        void ICompositionHost.ComposeParts(params object[] attributedParts)
        {
            _container.ComposeParts(attributedParts);
        }

        /// <inheritdoc />
        void ICompositionHost.AddToComposition(ComposablePartCatalog composablePartCatalog)
        {
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
        }

        /// <summary>
        /// Checks that an export gotten from the composition container is a non shared instance.
        /// </summary>
        /// <param name="type">The type being exported</param>
        private static void ValidateNonShared(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(PartCreationPolicyAttribute), true) as PartCreationPolicyAttribute;
            var creationPolicy = attribute != null ? attribute.CreationPolicy : CreationPolicy.Shared;
            if (creationPolicy != CreationPolicy.NonShared)
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
            if (!exportedObjects.Any())
            {
                return;
            }

            foreach (var exportedObject in exportedObjects)
            {
                var attribute =
                    Attribute.GetCustomAttribute(exportedObject.GetType(), typeof (PartCreationPolicyAttribute), true)
                        as PartCreationPolicyAttribute;
                if (attribute != null && attribute.CreationPolicy == CreationPolicy.NonShared)
                {
                    return;
                }
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
            var attribute =
                Attribute.GetCustomAttribute(type, typeof (PartCreationPolicyAttribute), true) as
                    PartCreationPolicyAttribute;
            var creationPolicy = attribute != null ? attribute.CreationPolicy : CreationPolicy.Shared;
            if (creationPolicy != CreationPolicy.Shared && creationPolicy != CreationPolicy.Any)
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
            foreach (var export in exportedObjects)
            {
                var attribute =
                Attribute.GetCustomAttribute(export.GetType(), typeof (PartCreationPolicyAttribute), true) as
                    PartCreationPolicyAttribute;
                var creationPolicy = attribute != null ? attribute.CreationPolicy : CreationPolicy.Shared;
                if (creationPolicy != CreationPolicy.Shared && creationPolicy != CreationPolicy.Any)
                {
                    throw new InvalidOperationException("MEF exported type: " + export.GetType().FullName + " is used by GetSharedExportedValues but it is not marked as shared.\n" +
                        "You need to add the \n\n[PartCreationPolicy(CreationPolicy.Shared)]\n\nAttribute to the type definition or use GetNonSharedExportedValues instead.");
                }
            }
        }

        #endregion
    }
}
