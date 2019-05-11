using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotosToVideo
{
    public class ImagePathComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var regex = new Regex(@"^.+\\(\d+)\.jpg$");

            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);

            if (xRegexResult.Success && yRegexResult.Success)
            {
                return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
            }

            return x.CompareTo(y);
        }
    }
}
