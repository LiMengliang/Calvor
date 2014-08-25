using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// Local file system view model.
    /// </summary>
    public class ImagesSelectorViewModel : INotifyPropertyChanged
    {
        #region Public Static Instance

        /// <summary>
        /// Singleton instance.
        /// </summary>
        // public static ImagesSelectorViewModel StaticInstance = new ImagesSelectorViewModel();

        #endregion

        #region Private Fields

        /// <summary>
        /// File system manager.
        /// </summary>
        private readonly ImageSelectorManager _fileSystemManager;

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

        private ICollection<Uri> _selectedImageUri;
        public ICollection<Uri> SelectedImagesUri
        {
            get { return _selectedImageUri; }
            set
            {
                _selectedImageUri = value;
                OnPropertyChanged("SelectedImagesUri");
            }
        }

        public void AddSelectedImagePath(Uri imageUri)
        {
            _fileSystemManager.AddSelectedImage(imageUri);
            SelectedImagesUri = _fileSystemManager.SelectedImagesPath;
        }

        public void RemoveSelectedIamgePath(Uri imageUri)
        {
            _fileSystemManager.DeselectedImage(imageUri);
            SelectedImagesUri = _fileSystemManager.SelectedImagesPath;
        }

        #endregion

        #region Constructors

        public ImagesSelectorViewModel()
        {
            _fileSystemManager = new ImageSelectorManager();
            FileSystemElements = from drive in DriveInfo.GetDrives() select new FileSystemElementViewModel(new Directory(drive.RootDirectory));
            SelectedImagesUri = new Collection<Uri>();
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
