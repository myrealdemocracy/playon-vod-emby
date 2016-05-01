﻿using System;
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
        public string Image { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string SeriesName { get; set; }
    }
}
