using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Emby.Scaffold
{
    public class Episode
    {
        public int? Number { get; set; }
        public List<Video> Videos { get; set; }
    }
}
