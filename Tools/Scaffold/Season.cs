﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Scaffold
{
    public class Season
    {
        public int? Number { get; set; }
        public List<SeriesVideo> Videos { get; set; }
    }
}