using System;
using System.ComponentModel;
using System.Windows.Media;
namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// File system element view model.
    /// </summary>
    public class FileSystemElementViewModel
    {
        #region Properties

        /// <summary>
        /// File system element.
        /// </summary>
        private readonly IFileSystemElement _fileSystemElement;
        public IFileSystemElement FileElement
        {
            get { return _fileSystemElement; }
        }

        /// <summary>
        /// Icon of the eleement.
        /// </summary>
        public ImageSource Icon
        {
            get { return _fileSystemElement.Icon; }
        }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string Name
        {
            get { return _fileSystemElement.Name; }
        }

        #endregion

        #region Constructors

        public FileSystemElementViewModel(IFileSystemElement fileSystemElement)
        {
            _fileSystemElement = fileSystemElement;
        }

        #endregion
    }
}
