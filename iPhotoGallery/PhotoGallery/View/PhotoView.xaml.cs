using System.Windows.Controls;
using PhotoGallery.ViewModel;

namespace PhotoGallery.View
{
    /// <summary>
    /// PhotoView.xaml 的交互逻辑
    /// </summary>
    public partial class PhotoView : UserControl
    {
        private PhotoGalleryViewModel _photoGalleryViewModel = PhotoGalleryViewModel.StaticInstance;

        public PhotoView()
        {
            InitializeComponent();
            _container.DataContext = _photoGalleryViewModel;
        }
    }
}
