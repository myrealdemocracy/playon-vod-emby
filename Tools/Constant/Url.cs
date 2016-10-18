using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Constant
{
    public class Url
    {
        public const string Base = "http://192.168.2.20:54479";
        public const string Xml = "/data/data.xml";
        public const string BaseRest = "http://192.168.3.40";
        public const string Video = BaseRest + "/url/video?id=";
        public const string Image = BaseRest + "/url/image?id=";

        public const string Omdb = "http://www.omdbapi.com/";
    }
}
