using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Calvor.PhotoGallery.FileBrowser.Model
{
    public class RootElement : IFileSystemElement
    {
        public static IFileSystemElement StaticRootElement = new RootElement();

        public FileElementType FileElementType
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public string FullName
        {
            get { throw new System.NotImplementedException(); }
        }

        public ImageSource Icon
        {
            get { throw new System.NotImplementedException(); }
        }

        public IFileSystemElement Parent
        {
            get { return StaticRootElement; }
        }

        public IEnumerable<IFileSystemElement> SubFileSystemElements
        {
            get
            {
                return from drive in DriveInfo.GetDrives()
                       select new Directory(drive.RootDirectory);
            }
        }
    }
}
