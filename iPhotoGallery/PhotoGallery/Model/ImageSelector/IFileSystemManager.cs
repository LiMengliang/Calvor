using System.Collections.Generic;
using System.IO;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// File system manager interface.
    /// </summary>
    public interface IFileSystemManager
    {
        /// <summary>
        /// Current element.
        /// </summary>
        IFileSystemElement CurrentElement { get; set; }
    }
}
