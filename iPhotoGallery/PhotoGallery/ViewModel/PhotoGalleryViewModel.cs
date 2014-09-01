using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Calvor.PhotoGallery.FileBrowser.Model;
using PhotoGallery.Model.Alubmn;

namespace PhotoGallery.ViewModel
{
    public class PhotoGalleryViewModel : INotifyPropertyChanged
    {
        public static PhotoGalleryViewModel StaticInstance = new PhotoGalleryViewModel();

        public ImagesSelectorViewModel ImagesSelectorViewModel;
        private AlbumnManager _albumnManager = new AlbumnManager();

        private ICollection<Albumn> _albumns;
        public ICollection<Albumn> Albumns
        {
            get { return _albumns; }
            set
            {
                _albumnManager.Albumns = value;
                _albumns = value;
                OnPropertyChanged("Albumns");
            }
        } 

        public PhotoGalleryViewModel()
        {
            ImagesSelectorViewModel = new ImagesSelectorViewModel();
            _fileSystemVisible = Visibility.Collapsed;
            Albumns = LocalAlbumn();
            ActiveAlbumn = Albumns.Count > 0 ? Albumns.First() : null;
        }

        private Albumn _activeAlbumn;
        public Albumn ActiveAlbumn
        {
            get { return _activeAlbumn; }
            set
            {
                _activeAlbumn = value;
                ImagesSelectorViewModel.ActiveAlbumn = value;
                OnPropertyChanged("ActiveAlbumn");
            }
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

        public ICollection<Albumn> LocalAlbumn()
        {
            return _albumnManager.LoadAlbumns();
        }

        public void SaveAlbumns()
        {
            _albumnManager.SaveAlbumns();
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
