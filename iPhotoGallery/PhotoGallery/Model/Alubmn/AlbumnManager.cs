using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace PhotoGallery.Model.Alubmn
{
    public class Albumn
    {

        private Uri _covePath;
        public Uri CoverImagePath 
        {
            get { return ImagePaths.Count > 0 ? ImagePaths.First() : null; }
        }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }
        public ICollection<Uri> ImagePaths { get; set; }

        public Albumn()
        {
            Guid = Guid.NewGuid();
            ImagePaths = new Collection<Uri>();
        }

        public Albumn(string name, string location, string time, ICollection<Uri> imagePaths, Guid guid)
        {
            Guid = guid;
            Name = name;
            Location = location;
            Time = time;
            ImagePaths = imagePaths;
        }
    }

    public class AlbumnManager
    {
        public ICollection<Albumn> Albumns { get; set; }

        // public AlbumnManager()
        // {
        //     Albumns = LoadAlbumns();
        // }
 
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
                        element.GetAttribute("Location"), imagePaths, new Guid(element.GetAttribute("ID")));
                    albumns.Add(albumn);
                }
            }
            catch { }
            return albumns;
        }

        public void SaveAlbumns()
        {
            var xmlDocumentation = new XmlDocument();
            try
            {
                xmlDocumentation.Load(@"E:\albumn.xml");
                XmlNode root = xmlDocumentation.SelectSingleNode("Albumns");
                root.RemoveAll();
                foreach (var albumn in Albumns)
                {
                    XmlElement albumnItem = xmlDocumentation.CreateElement("Albumn");
                    XmlAttribute albumnName = xmlDocumentation.CreateAttribute("Name");
                    albumnName.InnerText = albumn.Name;
                    XmlAttribute albumnTime = xmlDocumentation.CreateAttribute("Time");
                    albumnTime.InnerText = albumn.Time;
                    XmlAttribute albumnLocation = xmlDocumentation.CreateAttribute("Location");
                    albumnLocation.InnerText = albumn.Location;
                    XmlAttribute albumnID = xmlDocumentation.CreateAttribute("ID");
                    albumnID.InnerText = albumn.Guid.ToString();
                    albumnItem.SetAttributeNode(albumnID);
                    albumnItem.SetAttributeNode(albumnName);
                    albumnItem.SetAttributeNode(albumnTime);
                    albumnItem.SetAttributeNode(albumnLocation);
                    foreach (var imagePath in albumn.ImagePaths)
                    {
                        XmlElement image = xmlDocumentation.CreateElement("Image");
                        image.InnerText = imagePath.AbsolutePath;
                        albumnItem.AppendChild(image);
                    }
                    xmlDocumentation.DocumentElement.AppendChild(albumnItem);
                }
                xmlDocumentation.Save(@"E:\albumn.xml");
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
