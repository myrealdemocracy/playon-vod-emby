using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Scaffold
{
    public class Episode : Video
    {
        public int? EpisodeNumber { get; set; }
        public List<Video> Videos { get; set; }
    }
}
