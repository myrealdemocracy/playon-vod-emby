using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Constant
{
    public class Regex
    {
        public static List<string> SeasonEpisode = new List<string>
        {
            @"s(?<season>[0-9]+)e(?<episode>[0-9]+)\s*",
            @"season\s*(?<season>[0-9]+)\s*[|]*\s*episode\s*(?<episode>[0-9]+)\s*"
        };

        public static List<string> Season = new List<string>
        {
            ""
        };

        public static List<string> Episode = new List<string>
        {
            @"episode\s*(?<episode>[0-9]+)[:]*\s*",
            @"ep.\s*(?<episode>[0-9]+)[:]*\s*"
        };
    }
}
