using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Emby.Scaffold
{
    public class Video
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string SeriesName { get; set; }
        public string Overview { get; set; }
        public int? Minutes { get; set; }
        public bool IsLive { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Studios { get; set; }
        public string Image { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? ProductionYear { get; set; }
        public string OfficialRating { get; set; }
        public Dictionary<string, string> ProviderIds { get; set; }
    }
}
