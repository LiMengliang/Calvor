using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// Local file system view model.
    /// </summary>
    public class LocalFileSystemViewModel : INotifyPropertyChanged
    {
        #region Public Static Instance

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static LocalFileSystemViewModel StaticInstance = new LocalFileSystemViewModel();

        #endregion

        #region Private Fields

        /// <summary>
        /// File system manager.
        /// </summary>
        private readonly IFileSystemManager _fileSystemManager;

        #endregion

        #region Properties

        /// <summary>
        /// View model for all file system elements.
        /// </summary>
        public IEnumerable<FileSystemElementViewModel> FileSystemElements
        {
            get
            {
                return from element in _fileSystemManager.CurrentElement.SubFileSystemElements
                           select new FileSystemElementViewModel(element);
            }
            set
            {
                _fileSystemManager.CurrentElement = value.First().FileElement.Parent;
                OnPropertyChanged("FileSystemElements");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Nevigate into directory.
        /// </summary>
        /// <param name="fileSystemElement"></param>
        public void NevigateInto(FileSystemElementViewModel fileSystemElement)
        {
            _fileSystemManager.CurrentElement = fileSystemElement.FileElement;
            FileSystemElements = from fileElement in _fileSystemManager.CurrentElement.SubFileSystemElements
                                select new FileSystemElementViewModel(fileElement);
        }

        /// <summary>
        /// Nevigate out to parent directory.
        /// </summary>
        public void NevigateOut()
        {
            if (_fileSystemManager.CurrentElement == null)
            {
                return;
            }
            _fileSystemManager.CurrentElement = _fileSystemManager.CurrentElement.Parent;
            FileSystemElements = from fileElement in _fileSystemManager.CurrentElement.SubFileSystemElements
                                select new FileSystemElementViewModel(fileElement);
            // _fileSystemManager.NevigateOut(_fileSystemManager.CurrentElement);
            // FileSystemElements = from fileElement in _fileSystemManager.FileSystemElements
                                    // select new FileSystemElementViewModel(fileElement);
        }

        #endregion

        #region Constructors

        public LocalFileSystemViewModel()
        {
            _fileSystemManager = new LocalFileSystemManager();
            FileSystemElements = from drive in DriveInfo.GetDrives() select new FileSystemElementViewModel(new Directory(drive.RootDirectory));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
