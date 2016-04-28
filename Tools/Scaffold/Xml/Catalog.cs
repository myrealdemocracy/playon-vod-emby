using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PlayOn.Tools.Scaffold.Xml
{
    [Serializable]
    [XmlRoot(ElementName = "catalog")]
    public class Catalog
    {
        public Catalog() { Items = new List<Item>(); }

        [XmlElement("group")]
        public List<Item> Items { get; set; }
    }
}
