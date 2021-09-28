using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    class FileHandler
    {
        private string path = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure2\";

        public string ReadFile()
        {
            string fileName = $"{path}gk-api-dev-serviceagreements-api.template.json";
            // This text is added only once to the file.
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"The file {fileName} doesn't exist" );
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
            // data["resources"][0]["apiVersion"]
            // dynamic json2  = JsonConvert.DeserializeObject(dataString);
            
            // dynamic data2 = JObject.Parse(dataString);
            // string d = data2.resources[0].apiVersion;

            //TODO check null objects
            foreach (dynamic resource in data["resources"]) {
                // Move the API itself to another file
                switch (resource.type)
                {
                    case string s when s.StartsWith("Microsoft.ApiManagement/service/apis"):
                        MoveTheAPI(resource);
                }
                // Move the diagnostics to another file
                // Move the operations and their policies to another file
                // Move all other things to another file
                
                // maybe also add a replacer that replace add concat to the policies
                // maybe also add a replace that replace all LA parameters to something that use LA_general_info
                
                
                
                Console.WriteLine(resource.type);
            }









            Console.WriteLine("done reading JSON");

        }

        static void Main(string[] args)
        {

            FileHandler fh = new FileHandler();
            string data = fh.ReadFile();
            fh.HandleJson(data);
            
            Console.WriteLine("Hello World!");
            
        }
    }
}