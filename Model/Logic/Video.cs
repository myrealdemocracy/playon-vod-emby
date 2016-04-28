using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Model.Logic
{
    public class Video
    {
        public static void SaveAll()
        {
            foreach (var video in Tools.Helper.Video.All())
            {
                var isSeries = !String.IsNullOrEmpty(video.Series);
            }
        }
    }
}
