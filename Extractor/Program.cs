using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    class FileHandler
    {
        private string sourcePath;        
        private string destinationPath;
        private string templateFile;

        public FileHandler(string apiFileSourcePath)
        {
            this.sourcePath = apiFileSourcePath;
            this.templateFile = @"Template.json";
        }



        public string ReadFile()
        {
            string fileName = $"{sourcePath}gk-api-dev-serviceagreements-api.template.json";
            // This text is added only once to the file.
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"The file {fileName} doesn't exist");
            }  

            // This text is always added, making the file longer over time
            // if it is not deleted.
            // string appendText = "This is extra text" + Environment.NewLine;
            // File.AppendAllText(path, appendText, Encoding.UTF8);

            // Open the file to read from.
            string readText = File.ReadAllText(fileName);
            
            
            // Create a file to write to.
            // string createText = "Hello and Welcome" + Environment.NewLine;
            // File.WriteAllText(path, createText, Encoding.UTF8);
            
            // Console.WriteLine(readText);
            return readText;
        }

        public void HandleJson(string dataString)
        {
            JObject data = JObject.Parse(dataString);
            // string parametersString =  "";

            List<dynamic> theApi = new List<dynamic>();
            List<dynamic> operations = new List<dynamic>();
            List<dynamic> diagnostics = new List<dynamic>();
            List<dynamic> schemas = new List<dynamic>();
            
            List<dynamic> products = new List<dynamic>();
            List<dynamic> otherThings = new List<dynamic>();


            foreach (dynamic resource in data["resources"]) {
                switch (resource.type.ToString())
                {
                    //The Api (api & api policy):
                    case "Microsoft.ApiManagement/service/apis":
                        theApi.Add(resource);
                        break;
                    case "Microsoft.ApiManagement/service/apis/policies":
                        theApi.Add(resource);
                        break;
                    
                    // The schema:
                    case "Microsoft.ApiManagement/service/apis/schemas":
                        schemas.Add(resource);
                        break;
                    
                    // The operations (& their operations):
                    case string s when s.StartsWith("Microsoft.ApiManagement/service/apis/operations"):
                        operations.Add(resource);
                        break;
                    
                    // The diagnostics:
                    case string s when s.Contains("diagnostics"):
                        diagnostics.Add(resource);
                        break;
                    
                    // The products:
                    case string s when s.Contains("products"):
                        products.Add(resource);
                        break;
                    // Other stuff:
                    default:
                        otherThings.Add(resource);
                        break;

                }
                
                
                
                
                Console.WriteLine(resource.type);
            }
            
            // Move the API ('service/apis' + 'service/apis/policy')
            MoveTheAPI(theApi);
            // Move the diagnostics to another file
            MoveDiagnostic(diagnostics);
            // Move the operations and their policies to another file
            MoveOpertations(operations);
            //Move the schemas
            MoveSchemas(schemas);
            
            //Move the schemas
            MoveProducts(products);
            //Move the schemas
            MoveOtherThings(otherThings);
            
            // Move all other things to another file
            
            // maybe also add a replacer that replace add concat to the policies
            // maybe also add a replace that replace all LA parameters to something that use LA_general_info




            Console.WriteLine("done reading JSON");

        }

        
        private void MoveTheAPI(List<dynamic> list) // need APIM_name only
        {
            Console.WriteLine("In TheApi Handler");
            if (!File.Exists(this.templateFile))
            {
                Console.WriteLine($"The file {this.templateFile} doesn't exist" );
            } 
            
            string templateFile = File.ReadAllText(this.templateFile);
            dynamic template = JObject.Parse(templateFile);

            JObject result = new JObject();
            result.Add("$schema", template["$schema"]);
            result.Add("contentVersion", template.contentVersion);

            JObject parameters = new JObject();
            parameters.Add("ApimServiceName", template.parameters.ApimServiceName);
            parameters.Add("LAs_general_info", template.parameters.LAs_general_info);
            
            result.Add("parameters", parameters);

            JArray resources = new JArray(list);
            result.Add("resources", resources);
            

            string x = JsonConvert.SerializeObject(result);
            
            // if (!File.Exists("theApi.json"))
            // {
                // Console.WriteLine($"The file {fileName} doesn't exist");
                File.WriteAllText("theApi.json", x, Encoding.UTF8);
            // }  

            Console.WriteLine("End Of method");


        }
        
        private void MoveOpertations(List<object> list)
        {
            Console.WriteLine("In Operation Handler");
        }
        private void MoveDiagnostic(List<object> list)
        {
            Console.WriteLine("In diagnostics Handler");
        }
        private void MoveSchemas(List<object> list)
        {
            Console.WriteLine("In schemas Handler");
        }
        
        private void MoveProducts(List<object> list)
        {
            Console.WriteLine("In products Handler");
        }
        private void MoveOtherThings(List<object> list)
        {
            Console.WriteLine("In other stuff Handler");
        }

        static void Main(string[] args)
        {
            string apiPath =
                @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure2\";
            FileHandler fh = new FileHandler(apiPath);
            
            string data = fh.ReadFile();
            fh.HandleJson(data);
            
            Console.WriteLine("Hello World!");
            
        }
    }
}