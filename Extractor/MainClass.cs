using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class MainClass //TODO Find a better name
    {
        
        private Dictionary<ResourceTypes, JObject> _allResources;

        private readonly FileHandler _fh;
        private readonly Splitter _splitter;
        private readonly Replacer _replacer;


        public MainClass(string sourcePath, string destinationFolderPath, string yamlFile)
        {
            this._allResources = new Dictionary<ResourceTypes, JObject>();

            this._fh = new FileHandler(sourcePath, destinationFolderPath);
            
            _fh.ReadYamlFile(yamlFile);
            
            this._splitter = new Splitter();
            this._replacer = new Replacer();
        }

        public void Run()
        {
            
            string data = _fh.ReadApiJsonFile();
            _allResources = _splitter.SplitJson(data);
            
            JObject improvedLogger = _replacer.ReplaceInLoggerFile(_allResources[ResourceTypes.Logger]);
            JObject improvedOperations = _replacer.ReplaceInTheOperations(_allResources[ResourceTypes.Operations]);

            _allResources[ResourceTypes.Logger] = improvedLogger;
            _allResources[ResourceTypes.Operations] = improvedOperations;

            _fh.PrintAllFiles(_allResources);
            
            Console.WriteLine("Hello World!");
        }


        private static void Main(string[] args)
        {
            
            string sourceFile =
                @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure2\gk-api-dev-serviceagreements-api.template.json";
            string destinationPath = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure3";

            string yamlFile = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\azure-pipelines.yml";
            
            MainClass mc = new MainClass(sourceFile, destinationPath, yamlFile);

            mc.Run();


        }
    }
}