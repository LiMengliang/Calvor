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
        }

        private void CreateAlbumn_Click(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.FileSystemVisible = Visibility.Visible;
        }

        private void SaveAlbumn_OnClick(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.AddAlbumn(AlbumnName.Text, Time.Text, Location.Text, _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri);
        }
    }
}
