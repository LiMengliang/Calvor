using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calvor.SDK.PlatformFramework;

namespace SimpleWindowApplication
{
    public class PhotoGalleryApplication : IApplication
    {
        private string _name;
        public string Name
        {
            get { return "PhotoGallery"; }
            set { _name = value; }
        }
    }
}
