using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// File element.
    /// </summary>
    public class File : IFileSystemElement
    {
        #region Fileds

        /// <summary>
        /// File information.
        /// </summary>
        private FileInfo _fileInfo;

        #endregion

        #region IFileSystemElement Members

        /// <inheritdoc />
        public FileElementType FileElementType
        {
            get { return FileElementType.FileType; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _fileInfo.Name; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return _fileInfo.FullName; }
        }

        /// <inheritdoc />
        public File(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        /// <inheritdoc />
        public ImageSource Icon
        {
            get { return new BitmapImage(new Uri(FullName)); }
        }

        /// <inheritdoc />
        public IFileSystemElement Parent
        {
            get
            {
                return new Directory(_fileInfo.Directory);
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<IFileSystemElement> SubFileSystemElements
        {
            get { return new Collection<IFileSystemElement>(); }
        }

        #endregion
    }
}
