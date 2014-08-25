
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Calvor.PhotoGallery.FileBrowser.Model;
using PhotoGallery.ViewModel;

namespace Calvor.PhotoGallery.FileBrowser.View
{
    /// <summary>
    /// FileSystemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class SelectImagesView : UserControl
    {
        private ImagesSelectorViewModel _imageSelectorViewModel;
        private RoutedCommand _openFileCommand = new RoutedCommand("OpenFile", typeof(SelectImagesView));
        public SelectImagesView()
        {
            InitializeComponent();
            _imageSelectorViewModel = PhotoGalleryViewModel.StaticInstance.ImagesSelectorViewModel;
            _fileSystemView.DataContext = _imageSelectorViewModel;
            _selectedImages.DataContext = _imageSelectorViewModel;
        }

        private void _fileSystemView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _imageSelectorViewModel.NevigateInto(_fileSystemView.SelectedItem as FileSystemElementViewModel);
            }
            catch (Exception)
            {
            }
        }

        private void _goBack_OnClick(object sender, RoutedEventArgs e)
        {
            _imageSelectorViewModel.NevigateOut();
        }

        private void SelectImage(object sender, RoutedEventArgs e)
        {
            var fileElement = (FileSystemElementViewModel)((ListBoxItem)sender).Content;
            if (fileElement.FileElement is File)
            {
                _imageSelectorViewModel.AddSelectedImagePath(new Uri(fileElement.FileElement.FullName));
                _imageSelectorViewModel.SelectedImagesUri = new List<Uri>(_imageSelectorViewModel.SelectedImagesUri);
            }
        }

        private void DeselectImage(object sender, RoutedEventArgs e)
        {
            _imageSelectorViewModel.RemoveSelectedIamgePath((Uri)(((ListBoxItem)sender).Content));
                _imageSelectorViewModel.SelectedImagesUri = new List<Uri>(_imageSelectorViewModel.SelectedImagesUri);
        }
    }
}
