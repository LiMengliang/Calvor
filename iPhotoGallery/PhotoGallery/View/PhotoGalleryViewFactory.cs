using System.ComponentModel.Composition;
using System.Windows;
using Calvor.SDK.PlatformFramework.Shell;

namespace PhotoGallery.View
{
    [Export(typeof(IApplicationViewFactory))]
    public class PhotoGalleryViewFactory : IApplicationViewFactory
    {
        public UIElement CreateSideView()
        {
            return new AlbumnView();
        }

        public UIElement CreateMainView()
        {
            return new AlbumnPhotosView();
        }
    }
}
