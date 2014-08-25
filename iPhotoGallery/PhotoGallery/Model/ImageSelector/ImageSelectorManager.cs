using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    /// <summary>
    /// Local file system manager.
    /// </summary>
    public class ImageSelectorManager
    {
        #region Constructors

        public ImageSelectorManager()
        {
            CurrentElement = RootElement.StaticRootElement;
            SelectedImagesPath = new Collection<Uri>();
        }

        #endregion

        public IFileSystemElement CurrentElement { get; set; }

        public ICollection<Uri> SelectedImagesPath { get; set; }

        public void AddSelectedImage(Uri uri)
        {
            if (SelectedImagesPath.All(item => item != uri))
            {
                SelectedImagesPath.Add(uri);
            }
        }
        
        public void DeselectedImage(Uri uri)
        {
            if (SelectedImagesPath.Any(item => item == uri))
            {
                SelectedImagesPath.Remove(uri);
            }
        }
    }
}
