using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Scaffold
{
    public class Movie
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Video> Videos { get; set; }
    }
}
