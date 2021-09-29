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
        private readonly string _sourcePath;        
        private Mover mover;

        public FileHandler(string apiFileSourcePath, string destinationPath)
        {
            this._sourcePath = apiFileSourcePath;
            this.mover = new Mover(destinationPath);
        }



        public string ReadFile()
        {
            string fileName = $"{_sourcePath}";
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"The file {fileName} doesn't exist");
            }

            // Open the file to read from.
            string readText = File.ReadAllText(fileName);


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
                switch (resource["type"].ToString())
                {
                    //The Api (api & api policy):
                    case "Microsoft.ApiManagement/service/apis":  // leave the dependencies
                        theApi.Add(resource);
                        break;
                    case "Microsoft.ApiManagement/service/apis/policies": // leave the dependencies
                        theApi.Add(resource);
                        break;
                    
                    // The schema:
                    case "Microsoft.ApiManagement/service/apis/schemas":
                        resource["dependsOn"] = new JArray(); // remove the dependencies
                        schemas.Add(resource);
                        break;
                    
                    // The operations (& their operations):
                    case "Microsoft.ApiManagement/service/apis/operations":
                        resource["dependsOn"] = new JArray(); // remove the dependencies
                        operations.Add(resource);
                        break;
                    
                    case "Microsoft.ApiManagement/service/apis/operations/policies":
                        dynamic res = FixOperationPolicyDependencies(resource); // FIX the dependencies
                        operations.Add(res);
                        break;
                    
                    // The diagnostics:
                    case string s when s.Contains("diagnostics"):
                        resource["dependsOn"] = new JArray();
                        diagnostics.Add(resource);
                        break;
                    
                    // The products:
                    case string s when s.Contains("products"):
                        resource["dependsOn"] = new JArray();
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
            mover.MoveTheApi(theApi);
            // Move the diagnostics to another file
            mover.MoveDiagnostic(diagnostics);
            // Move the operations and their policies to another file
            mover.MoveOperations(operations);
            //Move the schemas
            mover.MoveSchemas(schemas);
            
            //Move the schemas
            mover.MoveProducts(products);
            //Move the schemas
            mover.MoveOtherThings(otherThings);
            
            // Move all other things to another file
            
            // maybe also add a replacer that replace add concat to the policies
            // maybe also add a replace that replace all LA parameters to something that use LA_general_info




            Console.WriteLine("done reading JSON");

        }

        private JObject FixOperationPolicyDependencies(dynamic resource)
        {
            JArray originalDependencies = resource["dependsOn"];
            JArray newDependencies = new JArray();
            
            foreach (dynamic dependency in originalDependencies)
            {
                string dependencyValue = (string) dependency.Value;
                if (dependencyValue.StartsWith("[resourceId('Microsoft.ApiManagement/service/apis/operations', parameters('ApimServiceName'),"))
                    newDependencies.Add(dependency);
            }

            resource["dependsOn"] = newDependencies; // Reset the dependencies
            return resource;
        }
    }
}