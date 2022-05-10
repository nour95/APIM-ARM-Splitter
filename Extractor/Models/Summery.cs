using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class Summery
    {
        public List<JObject> All { get; set; }
        
        public List<JObject> Matched { get; set; }
        public List<JObject> UnMatched { get; set; }

        public List<Operations> InnerDifferences { get; set; }

        public Summery(List<JObject> all, List<JObject> matched, List<JObject> unMatched, List<Operations> innerDifferences)
        {
            All = all;
            Matched = matched;
            UnMatched = unMatched;
            InnerDifferences = innerDifferences;
        }
    }
}