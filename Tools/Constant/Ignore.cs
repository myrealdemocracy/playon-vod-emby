using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Constant
{
    public class Ignore
    {
        public static List<string> Name = new List<string>
        {
            "clips",
            "video clips",
            "clips & extras",
            "episode highlights",
            "playback options",
            "backstage & interviews",
            "just for kids",
            "family",
            "kids",
            "recaps",
            "previews",
            "webisodes",
            "minisodes",
            "pbs newshour",
            "web exclusive",
            "interview",
            "my list",
            "suggestions for you",
            "popular on netflix",
            "new releases",
            "anime",
            "latino",
            "featured",
            "sports"
        };

        public static List<string> NameContains = new List<string>
        {
            "this folder contains no content",
            "s00",
            "e00"
        };

        public static List<string> NameStartsWith = new List<string>
        {
            "top picks for",
            "children & family"
        };

        public static List<string> NameEndsWith = new List<string>
        {
            "preview"
        };

        public static List<string> HrefContains = new List<string>
        {
            "playon",
            "playmark",
            "playlater"
        };
    }
}
