using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class MainClass //TODO Find a better name
    {
        
        private Dictionary<ResourceTypes, List<JObject>> _allResources;
        private JObject _replacementData ;

        private readonly FileHandler _fh;
        private readonly Splitter _splitter;
        private readonly Extractor _extractor;
        private Replacer replacer;

        //TODO find a solution to them later
        private readonly string _apiSourceFilePath;
        private readonly string _destinationFolderPath;
        private readonly string _replacementFilePath;


        public MainClass(string apiSourceFilePath, string destinationFolderPath, string replacementFilePath)
        {
            this._allResources = new Dictionary<ResourceTypes, List<JObject>>();

            this._fh = new FileHandler();
            this._splitter = new Splitter();
            this._extractor = new Extractor();
            
            // --- 
            this._apiSourceFilePath = apiSourceFilePath;
            this._destinationFolderPath = destinationFolderPath;
            this._replacementFilePath = replacementFilePath;

            
        }

        public void Run()
        {
            
            string apiData = _fh.ReadFileAsString(_apiSourceFilePath);
            _allResources = _splitter.SplitJson(apiData);
            
            // extractor
            List<JObject> requiredOperationAndPolicy = _extractor.GetOperationByName(_allResources[ResourceTypes.Operations], "objects-expforumcustrelvals");
            // todo have schemas too
            _fh.PrintJsonInFile(requiredOperationAndPolicy, _destinationFolderPath, "kunde");


            
            // // replacer:
            //
            // string replacementDataString = _fh.ReadFileAsString(_replacementFilePath);
            // _replacementData = JObject.Parse(replacementDataString);
            // replacer = new Replacer(_replacementData);
            //
            //
            // JObject improvedLogger = replacer.ReplaceInLoggerFile(_allResources[ResourceTypes.Logger]);
            // // JObject improvedOperations = replacer.ReplaceInTheOperations(_allResources[ResourceTypes.Operations]);
            // replacer.getPolicybyOperationName(_allResources[ResourceTypes.Operations], "budgets");
            //
            // _allResources[ResourceTypes.Logger] = improvedLogger;
            // // _allResources[ResourceTypes.Operations] = improvedOperations;

            
            
            
            
            // replacement is done
            // _fh.PrintAllFiles(_allResources, _destinationFolderPath);
            
            Console.WriteLine("Hello World!");
        }


        private static void Main(string[] args)
        {
            //TODO make one path that point to the folder only
            // string apiSourceFile =
            //     @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\Azure2\gk-api-dev-serviceagreements-api.template.json";
            // string destinationPath = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\Azure3";
            //
            // string replacementFile = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\yamlParameters.json";
            //
            // // string yamlFile = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\azure-pipelines.yml";
            
            string apiSourceFile =
                @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API%20Unit4%20%28backend%29\Azure2\gk-api-dev-unit4-v1-api.template.json";
            string destinationPath = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API%20Unit4%20%28backend%29\Azure2";
            
            string replacementFile = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\yamlParameters.json";
            
            // string yamlFile = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\MyTest\azure-pipelines.yml";
            
            MainClass mc = new MainClass(apiSourceFile, destinationPath, replacementFile );
            
            mc.Run();
            
            
            
            string operationName = "budgets";
            // string pattern = @"^\[concat\(parameters\('ApimServiceName'\), '/[a-zA-z0-9]/x12/policy/\]$";
            string pattern = @"^\[concat\(parameters\(\'ApimServiceName\'\), \'/([A-Za-z0-9-])+/" + operationName + @"/policy\'\)\]$";
            
            // string input = "[concat(parameters('ApimServiceName'), '/serviceagreements/x12/policy')]";
            // string input = "[concat(parameters('ApimServiceName'), '/nour-v1/x12/policy')]";
            string input = "[concat(parameters('ApimServiceName'), '/serviceagreements/budgets/policy')]";
            
            bool x = Regex.IsMatch(input, pattern);
            
            
            
            Console.WriteLine(x);



        }
    }
}































