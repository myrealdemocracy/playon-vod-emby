using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PlayOn.Tools.Scaffold.Xml
{
    [Serializable]
    [XmlRoot(ElementName = "group")]
    public class Video
    {
        public Video()
        {
            Item = new Item();
            Series = new Info();
            Description = new Info();
        }

        [XmlElement("media")]
        public Item Item { get; set; }

        [XmlElement("series")]
        public Info Series { get; set; }

        [XmlElement("description")]
        public Info Description { get; set; }

        [XmlElement("date")]
        public Info Date { get; set; }

        [XmlElement("time")]
        public Info Time { get; set; }
    }

    [Serializable]
    public class Info
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
