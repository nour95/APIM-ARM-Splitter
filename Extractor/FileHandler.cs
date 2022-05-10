using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Extractor
{
    class FileHandler
    {
        public FileHandler() { }
        
        public string ReadFileAsString(string sourcePath)
        {
            string fileName = $"{sourcePath}";
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"The file {fileName} doesn't exist");
            }

            // Open the file to read from.
            string readText = File.ReadAllText(fileName);

            return readText;
        }

        public void ReadYamlFile(string yamlFileName)
        {
            string yamlContent = File.ReadAllText(yamlFileName);
            string jsonString = ConvertYamlToJsonString(yamlFileName, yamlContent);

            //TODO continue here 


            Console.WriteLine("Hello from yaml reeader!");
        }

        public string ConvertYamlToJsonString(string yamlFileName, string yamlContent)
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new StringReader(yamlContent));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            if (yamlObject != null)
                return serializer.Serialize(yamlObject);
            else
                throw new FileLoadException($"There are problems with reading the file {yamlFileName} as yaml ");
        }

        // seems un efficient
        public void ReadYamlFile2(string yamlFile)
        {
            string readText = File.ReadAllText(yamlFile);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            Dictionary<Object, Object> yaml =
                (Dictionary<Object, Object>) deserializer.Deserialize(new StringReader(readText));

            var la_general = ((Dictionary<Object, Object>) yaml["variables"])["LAs_general_info"];


            Console.WriteLine("Hello from yaml reeader!");
        }


        public void PrintJsonInFile(List<JObject> resourcesToPrint, string destinationFolderPath, string fileName)
        {
            Directory.CreateDirectory(destinationFolderPath);
            // Add the content to a file
            StringBuilder sb = new StringBuilder();
            foreach (var resource in resourcesToPrint)
            {
                string resultToFile = JsonConvert.SerializeObject(resource, Formatting.Indented);
                sb.Append(resultToFile).Append(",\n\n");
            }

            File.WriteAllText(@$"{destinationFolderPath}/{fileName.ToLower()}-template.json", sb.ToString(),
                Encoding.UTF8);
        }

        public void PrintAllFiles(Dictionary<ResourceTypes, List<JObject>> allResources, string destinationFolderPath)
        {
            // Add the content to a file
            foreach (var resource in allResources)
            {
                //todo  need to fix this printingList may use JArray instead of lists??
                string resultToFile = JsonConvert.SerializeObject(resource.Value, Formatting.Indented);
                File.WriteAllText(@$"{destinationFolderPath}/{resource.Key.ToString().ToLower()}-template.json",
                    resultToFile, Encoding.UTF8);
            }
        }


        public void PrintInnerDifferencesInFile(List<Operations> AllOperations, string destinationFolder,
            string fileName)
        {
            Directory.CreateDirectory(destinationFolder);
            // Add the content to a file
            StringBuilder sb = new StringBuilder();
            foreach (var element in AllOperations)
            {
                // sb.Append($"\"In the new resource with name: {element.Name} and type: {element.Type}, " +
                          // $"you have the following changes: \"");
                // sb.Append('\n');
                          
                          
                string operationsList = JsonConvert.SerializeObject(element, Formatting.Indented);
                sb.Append(operationsList)
                    .Append('\n')
                    .Append("-*--*-*-*-*-*-*-*-*-*-*-*-")
                    .Append("\n\n");
            }

            File.WriteAllText(@$"{destinationFolder}/{fileName.ToLower()}-template.json", sb.ToString(),
                Encoding.UTF8);
        }
    }
}