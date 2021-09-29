using System.Resources;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class Replacer
    {
        public Replacer()
        {
        }

        public JObject ReplaceInLoggerFile(JObject loggerTemplate)
        {
            foreach (dynamic resource in loggerTemplate["resources"])
            {
                if (resource["properties"]["loggerId"] != null)
                {
                    resource.properties.loggerId =
                        "[concat(subscription().id, '/resourceGroups/', resourceGroup().name,'/providers/Microsoft.ApiManagement/service/', parameters('ApimServiceName'),'/loggers/', parameters('applicationInsight'))]";
                }
            }
            //TODO may remove the azure monitor from here too

            return loggerTemplate;
        }
        
        public JObject ReplaceInTheOperations(JObject operationTemplate)
        {
            foreach (dynamic resource in operationTemplate["resources"])
            {
                if (resource.type == "Microsoft.ApiManagement/service/apis/operations/policies")
                {
                    resource.properties.value = "[concat('" + resource.properties.value + "')]";
                    
                    // TODO check if it has a logic app --> then, replace it
                    CheckLogicAppsAndReplaceThem(resource);
                }
                
                
            }

            return operationTemplate;
        }

        private void CheckLogicAppsAndReplaceThem(JObject resource)
        {
            // throw new System.NotImplementedException();
        }
    }
}