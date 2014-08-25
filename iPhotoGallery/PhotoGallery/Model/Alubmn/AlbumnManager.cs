using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.DocumentStructures;
using System.Xml;
using System.Xml.Serialization;

namespace PhotoGallery.Model.Alubmn
{
    public class Albumn
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }
        public ICollection<Uri> ImagePaths { get; set; }

        public Albumn(string name, string location, string time, ICollection<Uri> imagePaths)
        {
            Name = name;
            Location = location;
            Time = time;
            ImagePaths = imagePaths;
        }
    }

    public class AlbumnManager
    {
        public ICollection<Albumn> Albumns { get; set; }

        public AlbumnManager()
        {
            Albumns = LoadAlbumns();
        }
 
        public ICollection<Albumn> LoadAlbumns()
        {
            var albumns = new Collection<Albumn>();
            var xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(@"E:\albumn.xml");
                XmlNodeList albumnNodeList = xmlDocument.SelectNodes("Albumns/Albumn");
                foreach (XmlNode node in albumnNodeList)
                {
                    var imagePaths = new Collection<Uri>();
                    XmlElement element = (XmlElement)node;
                    foreach (XmlNode image in element.ChildNodes)
                    {
                        imagePaths.Add(new Uri(image.InnerText));
                    }
                    var albumn = new Albumn(element.GetAttribute("Name"), element.GetAttribute("Time"),
                        element.GetAttribute("Location"), imagePaths);
                    albumns.Add(albumn);
                }
            }
            catch { }
            return albumns;
        }

        public void AddAlbumn(string name, string time, string location, ICollection<Uri> imagePaths)
        {
            Albumns.Add(new Albumn(name, location, time, imagePaths));
            var xmlDocumentation = new XmlDocument();
            try
            {
                xmlDocumentation.Load(@"E:\albumn.xml");
                XmlElement albumn = xmlDocumentation.CreateElement("Albumn");
                XmlAttribute albumnName = xmlDocumentation.CreateAttribute("Name");
                albumnName.InnerText = name;
                XmlAttribute albumnTime = xmlDocumentation.CreateAttribute("Time");
                albumnTime.InnerText = time;
                XmlAttribute albumnLocation = xmlDocumentation.CreateAttribute("Location");
                albumnLocation.InnerText = location;
                albumn.SetAttributeNode(albumnName);
                albumn.SetAttributeNode(albumnTime);
                albumn.SetAttributeNode(albumnLocation);
                foreach (var imagePath in imagePaths)
                {
                    XmlElement image = xmlDocumentation.CreateElement("Image");
                    image.InnerText = imagePath.AbsolutePath;
                    albumn.AppendChild(image);
                }
                xmlDocumentation.DocumentElement.AppendChild(albumn);
                xmlDocumentation.Save(@"E:\albumn.xml");
            }
            catch (Exception)
            {
                
            }
        }
    }
}
