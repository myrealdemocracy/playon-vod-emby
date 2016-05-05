using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;

namespace PlayOn.Emby.Scaffold
{
    public class ChannelList
    {
        public List<ChannelItemInfo> ChannelItemInfos { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
