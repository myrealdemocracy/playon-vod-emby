﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Scaffold
{
    public class Movie : BaseVideo
    {
        public List<MovieVideo> Videos { get; set; }
    }
}
