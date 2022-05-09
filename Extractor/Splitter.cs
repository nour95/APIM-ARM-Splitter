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
        private readonly string _templateFilePath;

        public Splitter()
        {
            this._templateFilePath = @"Template.json";
            
            if (!File.Exists(this._templateFilePath))
            {
                throw new FileNotFoundException($"The template file {this._templateFilePath} doesn't exist");
            } 

        }

         public Dictionary<ResourceTypes, List<JObject>> SplitJson(string dataString)
        {
            JObject data = JObject.Parse(dataString);
            // string parametersString =  "";

            List<JObject> theApi = new List<JObject>();
            List<JObject> operations = new List<JObject>();
            List<JObject> diagnostics = new List<JObject>();
            List<JObject> schemas = new List<JObject>();
            
            List<JObject> products = new List<JObject>();
            List<JObject> otherThings = new List<JObject>();


            foreach (JObject resource in data["resources"]) {
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
                
                
                Console.WriteLine(resource["type"]);
            }
            
            Dictionary<ResourceTypes, List<JObject>> allResources = new Dictionary<ResourceTypes, List<JObject>>();


            allResources.Add(ResourceTypes.Api, theApi);//GetTheApi(theApi)); // Get the API ('service/apis' + 'service/apis/policy')
            allResources.Add(ResourceTypes.Operations, operations); //GetOperations(operations));
            allResources.Add(ResourceTypes.Logger, diagnostics); //GGetDiagnostic(diagnostics));
            allResources.Add(ResourceTypes.Schemas, schemas); //GGetSchemas(schemas));
            
            allResources.Add(ResourceTypes.Products, products); //GGetProducts(products));
            allResources.Add(ResourceTypes.Others, otherThings); //GGetOtherThings(otherThings));
            
            Console.WriteLine("done splitting JSON");

            return allResources;
        }


          
         private JObject GetResource(string[] paramArr , List<dynamic> resourcesList)
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



        // The get functions
        private JObject GetTheApi(List<dynamic> resourcesList)
        {
            string[] paramArr = new string[] {"ApimServiceName"};
            return GetResource(paramArr, resourcesList);
        }
        
        private JObject GetOperations(List<object> resourcesList)
        {
            string[] paramArr = new string[] { "ApimServiceName", "LAs_general_info" };
            return GetResource(paramArr, resourcesList);
        }
        
        private JObject GetDiagnostic(List<object> resourcesList) 
        {
            string[] paramArr = new string[] { "ApimServiceName", "applicationInsight" };
            return GetResource(paramArr, resourcesList);        
        }
        
        private JObject GetSchemas(List<object> resourcesList)
        {
            string[] paramArr = new string[] { "ApimServiceName" };
            return GetResource(paramArr, resourcesList);
        }
        
        private JObject GetProducts(List<object> resourcesList)
        {
            string[] paramArr = new string[] { "ApimServiceName" };
            return GetResource(paramArr, resourcesList);
        }
        
        private JObject GetOtherThings(List<object> resourcesList)
        {
            string[] paramArr = new string[] { "ApimServiceName" };
            return GetResource(paramArr, resourcesList);
        }
        
       
        //-------------------------------------------
        // Helper methods:
        //-------------------------------------------

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