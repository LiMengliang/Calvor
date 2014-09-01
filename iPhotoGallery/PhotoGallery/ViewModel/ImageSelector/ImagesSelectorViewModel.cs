using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using PhotoGallery.Model.Alubmn;

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
        private readonly ImageSelectorManager _imageSelectorManager;

        public Albumn ActiveAlbumn { get; set; }

        #endregion

        public ImagesSelectorViewModel()
        {
            _imageSelectorManager = new ImageSelectorManager();
            // ActiveAlbumn = activeAlbumn;
            // SelectedImagesUri = activeAlbumn.ImagePaths;
            FileSystemElements = from drive in DriveInfo.GetDrives() select new FileSystemElementViewModel(new Directory(drive.RootDirectory));
            SelectedImagesUri = new Collection<Uri>();
        }

        #region Properties

        /// <summary>
        /// View model for all file system elements.
        /// </summary>
        public IEnumerable<FileSystemElementViewModel> FileSystemElements
        {
            get
            {
                return from element in _imageSelectorManager.CurrentElement.SubFileSystemElements
                           select new FileSystemElementViewModel(element);
            }
            set
            {
                _imageSelectorManager.CurrentElement = value.First().FileElement.Parent;
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
            _imageSelectorManager.CurrentElement = fileSystemElement.FileElement;
            FileSystemElements = from fileElement in _imageSelectorManager.CurrentElement.SubFileSystemElements
                                select new FileSystemElementViewModel(fileElement);
        }

        /// <summary>
        /// Nevigate out to parent directory.
        /// </summary>
        public void NevigateOut()
        {
            if (_imageSelectorManager.CurrentElement == null)
            {
                return;
            }
            _imageSelectorManager.CurrentElement = _imageSelectorManager.CurrentElement.Parent;
            FileSystemElements = from fileElement in _imageSelectorManager.CurrentElement.SubFileSystemElements
                                select new FileSystemElementViewModel(fileElement);
        }

        private ICollection<Uri> _selectedImageUri;
        public ICollection<Uri> SelectedImagesUri
        {
            get { return _selectedImageUri; }
            set
            {
                _imageSelectorManager.SelectedImagesPath = value;
                _selectedImageUri = value;
                if (ActiveAlbumn != null)
                {
                    ActiveAlbumn.ImagePaths = value;
                }
                OnPropertyChanged("SelectedImagesUri");
            }
        }

        public void AddSelectedImagePath(Uri imageUri)
        {
            _imageSelectorManager.AddSelectedImage(imageUri);
            // ActiveAlbumn.ImagePaths = new Collection<Uri>(ActiveAlbumn.ImagePaths.ToList());
            SelectedImagesUri = _imageSelectorManager.SelectedImagesPath;
        }

        public void RemoveSelectedIamgePath(Uri imageUri)
        {
            _imageSelectorManager.DeselectedImage(imageUri);
            ActiveAlbumn.ImagePaths = (from imagePath in ActiveAlbumn.ImagePaths
                where !imagePath.Equals(imageUri)
                select imagePath).ToList();
            SelectedImagesUri = _imageSelectorManager.SelectedImagesPath;
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
