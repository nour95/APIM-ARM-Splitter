using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Extractor
{
    public class Splitter
    {
        // private readonly string _destinationFolderPath;
        private readonly string _templateFilePath;

        public Splitter(string destinationFolderPath)
        {
            // this._destinationFolderPath = destinationFolderPath;
            this._templateFilePath = @"Template.json";
            
            if (!File.Exists(this._templateFilePath))
            {
                throw new FileNotFoundException($"The template file {this._templateFilePath} doesn't exist");
            } 

        }




        // need APIM_name only
        public JObject GetTheApi(List<dynamic> resourcesList)
        {
            // const string fileName = "api-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            return Move(paramArr, resourcesList);
        }
        
        public JObject GetOperations(List<object> resourcesList)
        {
            // const string fileName = "operations-template.json";
            string[] paramArr = new string[] { "ApimServiceName", "LAs_general_info" };

            return Move(paramArr, resourcesList);
        }
        public JObject GetDiagnostic(List<object> resourcesList) 
        {
            // const string fileName = "logger-template.json";
            string[] paramArr = new string[] { "ApimServiceName", "applicationInsight" };

            return Move(paramArr, resourcesList);        
        }
        public JObject GetSchemas(List<object> resourcesList)
        {
            // const string fileName = "schemas-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            return Move(paramArr, resourcesList);
        }
        
        public JObject GetProducts(List<object> resourcesList)
        {
            // const string fileName = "products-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            return Move(paramArr, resourcesList);
        }
        public JObject GetOtherThings(List<object> resourcesList)
        {
            // const string fileName = "others-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            return Move(paramArr, resourcesList);
        }
        
        
        public JObject Move(string[] paramArr , List<dynamic> resourcesList)
        {
            JObject result = new JObject();

            // Read the template file
            dynamic template = JObject.Parse(File.ReadAllText(this._templateFilePath));

            // add $schema and contentVersion to result  
            result.Add("$schema", template["$schema"]);
            result.Add("contentVersion", template.contentVersion);
            
            // create and add the parameters
            JObject parameters = CreateParameters(template, paramArr);
            result.Add("parameters", parameters);


            // Add the resources 
            JArray resources = new JArray(resourcesList);
            result.Add("resources", resources);
            
            return result;
            

        }

        
        // Helper methods:
        private JObject CreateParameters(dynamic template, string[] paramArr)
        {
            JObject parameters = new JObject();
            
            foreach (string param in paramArr)
            {
                parameters.Add(param, template.parameters[param]);

            }

            return parameters;
        }

    }
}