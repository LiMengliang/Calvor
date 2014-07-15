using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace Calvor.Core.Composition
{
    /// <summary>
    /// The CompositionHost represents and manages the object which makes up the entire Calvor application stack.
    /// One instance of the composition host manages and ties all plug-ins and framework components together.
    /// Please refer to the designer document which cover the next gen plug-in system for more information
    /// on the role of the composition host.
    /// </summary>
    public interface ICompositionHost
    {
        /// <summary>
        /// Creates an instance of a type that requires composition. The type can either be exported through
        /// composition or it can just have required imports.  It is also assumed that the type will be a 
        /// non-shared component and this assertion will be checked in debug builds and tests.
        /// </summary>
        /// <typeparam name="T">The type of object to create an instance of</typeparam>
        /// <returns>The created object</returns>
        T CreateInstance<T>() where T : class, new();

        /// <summary>
        /// Gets an object that is a MEF exported type.
        /// See the documentation for CompositionContainer.GetExportedValue for a full description.
        /// This method is a wrapper around CompositionContainer.GetExportedValue and should only be
        /// called on exported types that have a sharing policy of CreationPolicy.Shared or CreationPolicy.Any.
        /// Objects returned by this call will be cached so that subsequent retrievals of the shared object
        /// are much faster.
        /// </summary>
        /// <typeparam name="T">The type of object to get.  Must be MEF exported type</typeparam>
        /// <returns>
        /// The exported value.  An exception is thrown if there is no export available, if there
        /// is more than one export available, or if the export CreationPolicy is NonShared.
        /// </returns>
        T GetSharedExportedValue<T>();

        /// <summary>
        /// Gets an object that is a MEF exported type.
        /// See the documentation for CompositionContainer.GetExportedValue for a full description.
        /// This method is a direct wrapper of CompositionContainer.GetExportedValue and should only
        /// be called on exported types that have a sharing policy of CreationPolicy.NonShared.  You should
        /// only call this method if you are trying to create an interface type or a type that overrides the
        /// default constructor with an importing constructor.  Otherwise, calling CreateInstance is preferred.
        /// </summary>
        /// <typeparam name="T">The type of object to get.  Must be MEF exported type</typeparam>
        /// <returns>
        /// The exported value.  An exception is thrown if there is no export available, if there
        /// is more than one export available, or if the export CreationPolicy is Shared or Any.
        /// </returns>
        T GetNonSharedExportedValue<T>();

        /// <summary>
        /// Gets an object that is an MEF exported type.
        /// This method is identical to GetSharedExportedValue except it will return null instead of throwing exceptions
        /// if a valid export can't be found.
        /// </summary>
        /// <typeparam name="T">The type of object to get.  Must be MEF exported type</typeparam>
        /// <returns>The exported value or default(T) is there is not export available.</returns>
        T GetSharedExportedValueOrDefault<T>();

        /// <summary>
        /// Get multiple exported objects.  This is a direct wrapper of CompositionContainer.GetExportedValues() 
        /// and should only be called if all of the exported types in the enumeration has a sharing policy of 
        /// CreationPolicy.Shared or CreationPolicy.Any.  Objects returned by this call will be cached so that 
        /// subsequent retrievals of the shared objects are much faster.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Enumerable of exported objects.  An exception is thrown in debug builds and in tests if any of the 
        /// exported values don't have a sharing policy CreationPolicy.Shared or CreationPolicy.Any.
        /// </returns>
        IEnumerable<T> GetSharedExportedValues<T>();

        /// <summary>
        /// Get multiple exported objects.  This is a direct wrapper of CompositionContainer.GetExportedValues() 
        /// and should only be called if at least one of the exported types in the enumeration has a sharing 
        /// policy of CreationPolicy.NonShared.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Enumerable of exported objects.  An exception is thrown in debug builds and in tests if at least 
        /// one of the exported values doesn't have a sharing policy of CreationPolicy.NonShared.
        /// </returns>
        IEnumerable<T> GetNonSharedExportedValues<T>();

        /// <summary>
        /// Returns the exported object with the specified contract name. If there is not exactly one matching 
        /// exported object, an exception is thrown.  This is a direct wrapper for CompositionContainer.GetExportedValue.
        /// </summary>
        /// <typeparam name="T">The type of the exported object to return.</typeparam>
        /// <param name="contractName">
        /// The contract name of the exported object to return, or null or an empty string ("") to use 
        /// the default contract name.
        /// </param>
        /// <returns>The exported object with the specified contract name.</returns>
        T GetExportedValue<T>(string contractName);

        /// <summary>
        /// Get multiple exported objects including their metadata.  This is a direct wrapper of
        /// CompositionContainer.GetExports()
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <typeparam name="TMetadata">Type of export metadata</typeparam>
        /// <returns>Enumerable of lazy objects that lazily create the exported objects.</returns>
        IEnumerable<Lazy<T, TMetadata>> GetExports<T, TMetadata>();

        /// <summary>
        /// Get multiple exported objects.  This is a direct wrapper of CompositionContainer.GetExports().
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>Enumerable of lazy objects that lazily create the exported objects.</returns>
        IEnumerable<Lazy<T>> GetExports<T>();

        /// <summary>
        /// Get multiple exported objects.  This is a direct wrapper of CompositionContainer.GetExports().
        /// </summary>
        /// <param name="type">The type parameter of the objects to return.</param>
        /// <param name="metadataViewType">The type of the metadata view of the objects to return</param>
        /// <param name="contractName">
        /// The contract name of the object to return, or null or an empty string ("") to use 
        /// the default contract name.
        /// </param>
        /// <returns>A collection of all the objects for the contract matching contractName.</returns>
        IEnumerable<Lazy<object, object>> GetExports(Type type, Type metadataViewType, string contractName);

        /// <summary>
        /// Gets all the exports with the specified contract name.  This is a direct wrapper for 
        /// CompositionContainer.GetExports.
        /// </summary>
        /// <typeparam name="T">
        /// The type parameter of the objects to return. The contract name is also derived from this 
        /// type parameter.
        /// </typeparam>
        /// <typeparam name="TMetadataView">The type of the metadata view of the objects to return.</typeparam>
        /// <param name="contractName">
        /// The contract name of the objects to return, or null or an empty string ("") to use the 
        /// default contract name.
        /// </param>
        /// <returns>The objects with the specified contract name if found; otherwise, an empty IEnumerable.</returns>
        IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName);

        /// <summary>
        /// Gets all exports matching the given import definition.  This is a direct wrapper for CompositionContainer.GetExports
        /// </summary>
        /// <param name="definition">The import definition defining the criteria the exports need to meet</param>
        /// <returns>A collection of all of the exports meeting the criteria of the import definition</returns>
        IEnumerable<Export> GetExports(ImportDefinition definition); 
        
        /// <summary>
        /// Dynamically adds a ComposablePartCatalog to the Composition
        /// </summary>
        /// <param name="composablePartCatalog">The ComposablePartCatalog to add</param>
        void AddToComposition(ComposablePartCatalog composablePartCatalog);

        /// <summary>
        /// Adds or removes the parts in the specified System.ComponentModel.Composition.Hosting.CompositionBatch
        /// from the container and executes composition.  This is a direct wrapper around Compose.
        /// </summary>
        /// <param name="batch">
        /// Changes to the System.ComponentModel.Composition.Hosting.CompositionContainer to include during the composition.
        /// </param>
        void Compose(CompositionBatch batch);

        /// <summary>
        /// Composes the specified part with recomposition disabled.  This is a direct wrapper around 
        /// AttributedModelServices.SatisfyImportsOnce.
        /// </summary>
        /// <param name="attributedPart">The part to compose.</param>
        /// <returns>The composed part.</returns>
        ComposablePart SatisfyImportsOnce(object attributedPart);

        /// <summary>
        /// Creates composable parts from an array of attributed objects and composes them in 
        /// this ICompositionHost.  This is a direct wrapper around AttributedModelServices.ComposeParts.
        /// </summary>
        /// <param name="attributedParts">An array of attributed objects to compose.</param>
        void ComposeParts(params object[] attributedParts);
    }
}
