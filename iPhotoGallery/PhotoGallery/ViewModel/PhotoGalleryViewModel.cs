using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents.DocumentStructures;
using Calvor.PhotoGallery.FileBrowser.Model;
using PhotoGallery.Model.Alubmn;

namespace PhotoGallery.ViewModel
{
    public class PhotoGalleryViewModel : INotifyPropertyChanged
    {
        public static PhotoGalleryViewModel StaticInstance = new PhotoGalleryViewModel();

        public ImagesSelectorViewModel ImagesSelectorViewModel = new ImagesSelectorViewModel();
        private AlbumnManager _albumnManager = new AlbumnManager();

        public ICollection<Albumn> Albumns { get; set; } 

        public PhotoGalleryViewModel()
        {
            _fileSystemVisible = Visibility.Collapsed;
            ImagesSelectorViewModel = new ImagesSelectorViewModel();
            Albumns = LocalAlbumn();
        }

        private Visibility _fileSystemVisible;
        public Visibility FileSystemVisible 
        {
            get { return _fileSystemVisible; }
            set
            {
                _fileSystemVisible = value ;
                OnPropertyChanged("FileSystemVisible");
            }
        }

        public void AddAlbumn(string name, string time, string location, ICollection<Uri> imagePaths)
        {
            _albumnManager.AddAlbumn(name, time, location, imagePaths);
        }

        public ICollection<Albumn> LocalAlbumn()
        {
            return _albumnManager.LoadAlbumns();
        }

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
