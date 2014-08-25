using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    public enum FileElementType
    {
        DirectoryType,
        FileType
    }

    /// <summary>
    /// File system element interface.
    /// </summary>
    public interface IFileSystemElement
    {
        /// <summary>
        /// Type information.
        /// </summary>
        FileElementType FileElementType { get; }

        /// <summary>
        /// Short name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Full name.
        /// </summary>
        string FullName { get; }
        
        /// <summary>
        /// Icon of the element.
        /// </summary>
        ImageSource Icon { get; }

        /// <summary>
        /// Parent of the element.
        /// </summary>
        IFileSystemElement Parent { get; }

        /// <summary>
        /// File system elements.
        /// </summary>
        IEnumerable<IFileSystemElement> SubFileSystemElements { get; }
    }
}
