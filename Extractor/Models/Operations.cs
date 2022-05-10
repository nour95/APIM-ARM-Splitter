using System.Collections.Generic;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class Operations
    {
        public Operations(){}

        public Operations(string name, string type, List<OneOperation> list)
        {
            this.Name = name;
            this.Type = type;
            this.OperationsList = list;
        }

        public Operations(string name, string type, List<OneOperation> operationsList, JObject newResource, JObject oldResource)
        {
            NewResource = newResource;
            OldResource = oldResource;
            Name = name;
            Type = type;
            OperationsList = operationsList;
        }

        [JsonIgnore]
        public JObject NewResource { get; set; }
        
        [JsonIgnore]
        public JObject OldResource { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("OperationsList")]
        public List<OneOperation> OperationsList { get; set; }
    }

    public class OneOperation
    {

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("op")]
        public string Op { get; set; }

        [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore) ]
        public string From { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public object Value { get; set; }
    }
    
    
    
}