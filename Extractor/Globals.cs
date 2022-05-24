using System.IO;

namespace Extractor
{
    public enum Types
    {
        ApiTags, ApiVersionSets, Tags , Api
    }
    public class Globals
    {
        public static readonly string[] NeededTypes = new[]{"apiTags", "apiVersionSets", "tags" , "api"};
        
        public static readonly string MainPath =  Path.Combine("C:","Users","NourAl-HudaAl-Majni","Desktop","nour","ibiz",
            "A1","gitRepos","shared","Shared.APIM");
        
        // The src folder that contain the older/first ARM template for the same API
        public static readonly string OldSrcFolder   = Path.Combine(MainPath, "APIM","D365");

        
        // the src folder that contain the newer/second ARM template for the same API
        public static readonly string SrcFolder      = Path.Combine(MainPath, "Debug", "d365_2");
        // Where to save the results of comparison?
        public static readonly string DestFolder = Path.Combine(MainPath, "Debug","results_2");
        
        
        public static readonly Types[] NeededTypes2 = new[]{Types.ApiTags, Types.ApiVersionSets, Types.Tags, Types.Api};

        
        
    }
}