using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PlayOn.Tools.Scaffold.Xml
{
    [Serializable]
    public class Item
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("searchable")]
        public string Searchable { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("src")]
        public string Src { get; set; }

        [XmlAttribute("art")]
        public string Art { get; set; }
    }
}
