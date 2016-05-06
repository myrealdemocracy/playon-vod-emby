using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;

namespace PlayOn.Emby.Scaffold.Result
{
    public class Channel
    {
        public List<ChannelItemInfo> Items { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
