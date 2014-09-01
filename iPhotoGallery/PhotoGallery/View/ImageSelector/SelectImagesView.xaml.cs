
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calvor.PhotoGallery.FileBrowser.Model;
using PhotoGallery.Model.Alubmn;
using PhotoGallery.ViewModel;

namespace Calvor.PhotoGallery.FileBrowser.View
{
    /// <summary>
    /// FileSystemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class SelectImagesView : UserControl
    {
        private PhotoGalleryViewModel _photoGalleryViewModel;
        public SelectImagesView()
        {
            InitializeComponent();
            _photoGalleryViewModel = PhotoGalleryViewModel.StaticInstance;
            _fileSystemView.DataContext = _photoGalleryViewModel.ImagesSelectorViewModel;
            _selectedImages.DataContext = _photoGalleryViewModel.ImagesSelectorViewModel;
        }

        private void _fileSystemView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _photoGalleryViewModel.ImagesSelectorViewModel.NevigateInto(_fileSystemView.SelectedItem as FileSystemElementViewModel);
            }
            catch (Exception)
            {
            }
        }

        private void _goBack_OnClick(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.ImagesSelectorViewModel.NevigateOut();
        }

        private void SelectIamge(object sender, RoutedEventArgs e)
        {
            var fileElement = (FileSystemElementViewModel)((ListBoxItem)sender).Content;
            if (fileElement.FileElement is File)
            {
                _photoGalleryViewModel.ImagesSelectorViewModel.AddSelectedImagePath(new Uri(fileElement.FileElement.FullName));
                _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri = new List<Uri>(_photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri);
            }
        }

        private void DeselectImage(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.ImagesSelectorViewModel.RemoveSelectedIamgePath((Uri)(((ListBoxItem)sender).Content));
            _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri = new List<Uri>(_photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri);
        }

        private void Save_OnClicked(object sender, RoutedEventArgs e)
        {
            _photoGalleryViewModel.FileSystemVisible = Visibility.Collapsed;
            _photoGalleryViewModel.Albumns = new Collection<Albumn>(_photoGalleryViewModel.Albumns.ToList());
            var temp = _photoGalleryViewModel.ActiveAlbumn;
            _photoGalleryViewModel.ActiveAlbumn = new Albumn();
            _photoGalleryViewModel.ActiveAlbumn = temp;

            _photoGalleryViewModel.SaveAlbumns();
        }

        private void SelectAllCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in _fileSystemView.Items)
            {
                var image = item as FileSystemElementViewModel;
                if (image != null && image.FileElement.FileElementType == FileElementType.FileType && image.FileElement.FullName.Contains(".jpg"))
                {
                    _fileSystemView.SelectedItems.Add(item);
                }
            }
        }

        private void ComfirmSelectedIamges_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in _fileSystemView.SelectedItems)
            {
                _photoGalleryViewModel.ImagesSelectorViewModel.AddSelectedImagePath(
                    new Uri((item as FileSystemElementViewModel).FileElement.FullName));
                _photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri =
                    new List<Uri>(_photoGalleryViewModel.ImagesSelectorViewModel.SelectedImagesUri);
            }
        }
    }
}
