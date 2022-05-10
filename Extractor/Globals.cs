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
        
        
        string sourceFolder      = Path.Combine(MainPath, "Nour", "d365");
        string destinationFolder = Path.Combine(MainPath, "Nour","results");

        string oldSourceFolder   = Path.Combine(MainPath, "APIM","D365");
        
        
        
        public static readonly Types[] NeededTypes2 = new[]{Types.ApiTags, Types.ApiVersionSets, Types.Tags, Types.Api};

        
        
    }
}