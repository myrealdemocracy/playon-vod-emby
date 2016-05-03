using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Scaffold
{
    public class Video
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int? Minutes { get; set; }
        public string SeriesName { get; set; }
    }
}
