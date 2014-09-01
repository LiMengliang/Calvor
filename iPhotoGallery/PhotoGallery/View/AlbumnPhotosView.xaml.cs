using System.Windows;
using System.Windows.Controls;
using PhotoGallery.Model.Alubmn;
using PhotoGallery.ViewModel;

namespace PhotoGallery.View
{
    /// <summary>
    /// PhotoView.xaml 的交互逻辑
    /// </summary>
    public partial class AlbumnPhotosView : UserControl
    {
        private PhotoGalleryViewModel _photoGalleryViewModel = PhotoGalleryViewModel.StaticInstance;

        public AlbumnPhotosView()
        {
            InitializeComponent();
            _container.DataContext = _photoGalleryViewModel;
        }

        private void Modify_OnClick(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.FileSystemVisible = Visibility.Visible;
            _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri = _photoGalleryViewModel.ActiveAlbumn.ImagePaths;
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = _photoGalleryViewModel.ActiveAlbumn;
            _photoGalleryViewModel.ActiveAlbumn = new Albumn();
            _photoGalleryViewModel.ActiveAlbumn = temp;
        }
    }
}
