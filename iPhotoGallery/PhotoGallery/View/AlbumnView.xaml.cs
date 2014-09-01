using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PhotoGallery.Model.Alubmn;
using PhotoGallery.ViewModel;

namespace PhotoGallery.View
{
    /// <summary>
    /// AlbumnView.xaml 的交互逻辑
    /// </summary>
    public partial class AlbumnView : UserControl
    {
        private PhotoGalleryViewModel _photoGalleryViewModel = PhotoGalleryViewModel.StaticInstance;
        

        public AlbumnView()
        {
            InitializeComponent();
            AlbumnViewList.DataContext = _photoGalleryViewModel;
        }

        private void CreateAlbumn_Click(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.FileSystemVisible = Visibility.Visible;
            _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri = new Collection<Uri>();
            _photoGalleryViewModel.ActiveAlbumn = new Albumn();
            // _photoGalleryViewModel.ImagesSelectorViewModel.ActiveAlbumn = _photoGalleryViewModel.ActiveAlbumn;
            _photoGalleryViewModel.Albumns.Add(_photoGalleryViewModel.ActiveAlbumn);
        }
    }
}
