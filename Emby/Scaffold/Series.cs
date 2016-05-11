using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;

namespace PlayOn.Emby.Scaffold
{
    public class Series : Video
    {
        public List<Season> Seasons { get; set; }
    }
}
