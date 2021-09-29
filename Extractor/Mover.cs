using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Extractor
{
    public class Mover
    {
        private readonly string _destinationFolderPath;
        private readonly string _templateFilePath;

        public Mover(string destinationFolderPath)
        {
            this._destinationFolderPath = destinationFolderPath;
            this._templateFilePath = @"Template.json";
            
            if (!File.Exists(this._templateFilePath))
            {
                throw new FileNotFoundException($"The template file {this._templateFilePath} doesn't exist");
            } 

        }




        // need APIM_name only
        public void MoveTheApi(List<dynamic> resourcesList)
        {
            const string fileName = "api-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            Move(fileName, paramArr, resourcesList);
        }
        
        public void MoveOperations(List<object> resourcesList)
        {
            const string fileName = "operations-template.json";
            string[] paramArr = new string[] { "ApimServiceName", "LAs_general_info" };

            Move(fileName, paramArr, resourcesList);
        }
        public void MoveDiagnostic(List<object> resourcesList) //TODO  need to have variables
        {
            Console.WriteLine("In diagnostics Handler");
        }
        public void MoveSchemas(List<object> resourcesList)
        {
            const string fileName = "schemas-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            Move(fileName, paramArr, resourcesList);
        }
        
        public void MoveProducts(List<object> resourcesList)
        {
            const string fileName = "products-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            Move(fileName, paramArr, resourcesList);
        }
        public void MoveOtherThings(List<object> resourcesList)
        {
            const string fileName = "others-template.json";
            string[] paramArr = new string[] { "ApimServiceName" };

            Move(fileName, paramArr, resourcesList);
        }
        
        
        public void Move(string fileName, string[] paramArr , List<dynamic> resourcesList)
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
            
            // Add the content to a file
            string resultToFile = JsonConvert.SerializeObject(result);
            File.WriteAllText(@$"{_destinationFolderPath}/{fileName}", resultToFile, Encoding.UTF8);
            

        }

        
        // Helper methods:
        private void AddEssentials(dynamic template, JObject result)
        {
            
        }
        
        
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