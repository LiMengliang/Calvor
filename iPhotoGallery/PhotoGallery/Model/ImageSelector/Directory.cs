using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// Directory model.
    /// </summary>
    public class Directory : IFileSystemElement
    {
        #region Private Fields

        /// <summary>
        /// Directory information.
        /// </summary>
        private DirectoryInfo _directoryInfo;

        #endregion

        #region Constructors

        public Directory(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        #endregion

        #region IFileSystemElement members

        /// <inheritdoc />
        public FileElementType FileElementType
        {
            get { return FileElementType.DirectoryType; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _directoryInfo.Name; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return _directoryInfo.FullName; }
        }

        /// <inheritdoc />
        public ImageSource Icon
        {
            get
            {
                return new BitmapImage(new Uri(@"E:\Calvor\Everything\iPhotoGallery\FileBrowser\Resource\Folder.png"));
            }
        }

        /// <inheritdoc />
        public IFileSystemElement Parent
        {
            get { return _directoryInfo.Parent == null ? RootElement.StaticRootElement : new Directory(_directoryInfo.Parent); }
        }

        /// <inheritdoc />
        public IEnumerable<IFileSystemElement> SubFileSystemElements
        {
            get
            {
                try
                {
                    var fileSystemItems = from directory in _directoryInfo.GetDirectories()
                                      select new Directory(directory) as IFileSystemElement;
                    fileSystemItems = fileSystemItems.Concat(from file in _directoryInfo.GetFiles()
                                                         select new File(file) as IFileSystemElement);
                    return fileSystemItems;

                }
                catch (Exception)
                {
                    throw new IOException("Can't nevigate into that directory.");
                }
            }
        }

        #endregion
    }
}
