using System.Resources;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;


namespace Extractor
{
    public class Replacer
    {
        // private Regex rgx = new Regex(@"^[a-zA-Z0-9]\d{2}[a-zA-Z0-9](-\d{3}){2}[A-Za-z0-9]$");

        private readonly JObject _replacementData;
        
        public Replacer(JObject replacementData)
        {
            this._replacementData = replacementData;
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

        public JObject getPolicybyOperationName(JObject operationTemplate, string operationName)
        {
            string pattern = @"^\[concat\(parameters\(\'ApimServiceName\'\), \'/([A-Za-z0-9-])+/" + operationName + @"/policy\'\)\]$";
            string linkToReplace = ""; //TODO I am here

            string recourseName;
            foreach (dynamic resource in operationTemplate["resources"])
            {
                recourseName = resource.name.ToString();
                if (resource.type == "Microsoft.ApiManagement/service/apis/operations/policies" 
                    &&  Regex.IsMatch(recourseName, pattern))
                {

                    // string y = resource.name.ToString();
                    // bool x4 = Regex.IsMatch(y, pattern);
                    XElement purchaseOrder = XElement.Load(resource.properties.value.ToString());

                    // resource.properties.value = "[concat('" + resource.properties.value + "')]";
                    
                    // TODO check if it has a logic app --> then, replace it
                    CheckLogicAppsAndReplaceThem(resource);
                }
                
                
            }
            return null;
        }

        private void CheckLogicAppsAndReplaceThem(JObject resource)
        {
            // throw new System.NotImplementedException();
        }
    }
}