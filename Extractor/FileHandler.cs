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


        public static string[] GetNameOfAllFilesInDirectory(string directoryName)
        {
            return Directory.GetFiles(directoryName);
        }
        
        public static string ReadFileAsString(string sourcePath)
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
        
        public static void CopyFile(string sourceFileName, string destFolder, string destFileName, bool overwrite)
        {
            // create the destination folder if not exist.
            Directory.CreateDirectory(destFolder);
            
            string newFileName = Path.Combine(destFolder, destFileName);
            File.Copy(sourceFileName, newFileName, overwrite);
        }

        public static void PrintJsonObjectInFile(JObject FileToPrint, string destFolder, string fileType)
        {
            Directory.CreateDirectory(destFolder);
            
            // Add the content to the file
            StringBuilder sb = new StringBuilder();
            
            string resultToFile = JsonConvert.SerializeObject(FileToPrint, Formatting.Indented);
            sb.Append(resultToFile).Append(",\n\n");
            

            File.WriteAllText(@$"{destFolder}/{fileType.ToLower()}.json", sb.ToString(),
                Encoding.UTF8);
        }
        
        public static void PrintJsonListInFile(List<JObject> resourcesToPrint, string destFolder, string fileType)
        {
            Directory.CreateDirectory(destFolder);
            
            // Add the content to the file
            StringBuilder sb = new StringBuilder();
            foreach (var resource in resourcesToPrint)
            {
                string resultToFile = JsonConvert.SerializeObject(resource, Formatting.Indented);
                sb.Append(resultToFile).Append(",\n\n");
            }

            File.WriteAllText(@$"{destFolder}/{fileType.ToLower()}.json", sb.ToString(),
                Encoding.UTF8);
        }


        public static void PrintInnerDifferencesInFile(List<Operations> allOperations, string destFolder,
            string fileType)
        {
            Directory.CreateDirectory(destFolder);
            // Add the content to a file
            StringBuilder sb = new StringBuilder();
            foreach (var element in allOperations)
            {
                
                string operationsList = JsonConvert.SerializeObject(element, Formatting.Indented);
                sb.Append(operationsList)
                    .Append('\n')
                    .Append("-*--*-*-*-*-*-*-*-*-*-*-*-")
                    .Append("\n\n");
            }

            File.WriteAllText(@$"{destFolder}/{fileType.ToLower()}.json", sb.ToString(),
                Encoding.UTF8);
        }


        
    }
}