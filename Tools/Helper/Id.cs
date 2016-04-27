using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Id
    {
        public static string Mapper(string href)
        {
            if (!href.Contains("=")) return href;

            var id = href.Split(Convert.ToChar("="))[1];

            if (id.Contains("-"))
            {
                id = id.Split(Convert.ToChar("-"))[0];
            }

            return id;
        }
    }
}
