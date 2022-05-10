using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class MainClass //TODO Find a better name
    {
        private static string mainPath = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\A1\gitRepos\shared\Shared.APIM";
        private readonly string[] neededTypes = new[]{"apiTags", "apiVersionSets", "tags", "globalServicePolicy" , "api"};

        private Dictionary<ResourceTypes, List<JObject>> _allResources;
        private JObject _replacementData ;

        private readonly FileHandler _fh;
        private readonly Splitter _splitter;
        private readonly Extractor _extractor;
        private Replacer replacer;

        //TODO find a solution to them later
        private readonly string sourceFolder;
        private readonly string destinationFolder;
        private readonly string oldSourceFolder;


        public MainClass(string sourceFolder, string destinationFolder, string oldSourceFolder)
        {
            this._allResources = new Dictionary<ResourceTypes, List<JObject>>();

            this._fh = new FileHandler();
            this._splitter = new Splitter();
            this._extractor = new Extractor();

            this.sourceFolder = sourceFolder;
            this.destinationFolder = destinationFolder;
            this.oldSourceFolder = oldSourceFolder;
        }
        private static void Main(string[] args)
        {
            //TODO make one path that point to the folder only
            string sourceFolder =  mainPath + @"\Nour\d365";
            string destinationFolder = mainPath + @"\Nour\results";

            string oldSourceFolder = mainPath + @"\APIM\D365";
            
            MainClass mc = new MainClass(sourceFolder, destinationFolder, oldSourceFolder );

            mc.Run();
            //
            //
            //
            // string operationName = "budgets";
            // // string pattern = @"^\[concat\(parameters\('ApimServiceName'\), '/[a-zA-z0-9]/x12/policy/\]$";
            // string pattern = @"^\[concat\(parameters\(\'ApimServiceName\'\), \'/([A-Za-z0-9-])+/" + operationName + @"/policy\'\)\]$";
            //
            // // string input = "[concat(parameters('ApimServiceName'), '/serviceagreements/x12/policy')]";
            // // string input = "[concat(parameters('ApimServiceName'), '/nour-v1/x12/policy')]";
            // string input = "[concat(parameters('ApimServiceName'), '/serviceagreements/budgets/policy')]";
            //
            // bool x = Regex.IsMatch(input, pattern);
            //
            //
            //
            // Console.WriteLine(x);



        }

        public void Run()
        {
            Dictionary<string, JObject> oldFilesMap  = getNeededTyesWithContents(oldSourceFolder, destinationFolder+@"2\");
            Dictionary<string, JObject> newFilesMap  = getNeededTyesWithContents(sourceFolder, destinationFolder+ @"\");

            //foreach (var type in neededTypes)
            string type = neededTypes[0];
            {
                // (var a, var s, var f) = compare(oldFilesMap[type], newFilesMap[type]);
                Summery s = compare(oldFilesMap[type], newFilesMap[type]);

                (_, _, var unMatched, var innerDiff) = 
                                    (s.All, s.Matched, s.UnMatched, s.InnerDifferences);
                _fh.PrintJsonInFile(unMatched, destinationFolder + @"_newResources\", type);
                
                _fh.PrintInnerDifferencesInFile(innerDiff, destinationFolder + @"_innerDifferences\", type);

            }

            Console.WriteLine("Hello World!");
        }

        private Summery compare(JObject oldFile, JObject newFile)
        {
            List<JObject> all = new List<JObject>();
            List<JObject> matched = new List<JObject>();
            List<JObject> unMatched = new List<JObject>();

            List<Operations> innerDifferences = new List<Operations>();

            bool found;
            foreach (JObject resourceInNew in newFile["resources"])
            {
                all.Add(resourceInNew);
                string newResourceName = resourceInNew["name"].ToString();
                string newResourceType = resourceInNew["type"].ToString();
                found = false;

                foreach (JObject resourceInOld in oldFile["resources"])
                {
                    string oldResourceName = resourceInOld["name"].ToString();
                    string oldResourceType = resourceInOld["type"].ToString();
                    if (oldResourceName == newResourceName && oldResourceType == newResourceType)
                    {
                        matched.Add(resourceInNew);
                        found = true;

                        List<OneOperation> comparedResult = compareContents(resourceInOld, resourceInNew);
                        // Operations operationsList = new Operations(newResourceName, newResourceType, comparedResult);
                        Operations operationsList = new Operations(newResourceName, newResourceType, comparedResult,
                            resourceInNew, resourceInOld);

                        innerDifferences.Add(operationsList);

                       // _fh.printComparedResults(comparedResult, fileType);
                        break;
                    }
                }
                if(! found)
                    unMatched.Add(resourceInNew);
            }
            
            return new Summery(all, matched, unMatched, innerDifferences);



        }

        private List<OneOperation> compareContents(JObject resourceInOld, JObject resourceInNew)
        {
            var jdp = new JsonDiffPatch();

            JToken patch = jdp.Diff(resourceInOld, resourceInNew);
            var output = jdp.Patch(resourceInNew, patch);
            var formatter = new JsonDeltaFormatter();
            var operations = formatter.Format(patch);

            var operationString = JsonConvert.SerializeObject(operations,Formatting.Indented);
            var operationsList = JsonConvert.DeserializeObject<List<OneOperation>>(operationString);

            // StringBuilder sb = new StringBuilder();
            //
            // sb.Append($"In the new resource with name: {resourceInNew["name"]} and type: {resourceInNew["type"]}, " +
            //                   $"you have the following changes:");
            // sb.Append("\n");
            //
            // // Console.WriteLine(patch.ToString());            
            // // Console.WriteLine("-*--*-*-*-*-*-*-*-*-*-*-*-");
            //
            // // Console.WriteLine(output.ToString());
            // // Console.WriteLine("-*--*-*-*-*-*-*-*-*-*-*-*-");
            //
            // sb.Append(operationString).Append("\n");      
            // sb.Append("-*--*-*-*-*-*-*-*-*-*-*-*-").Append("\n");
            
            // return sb.ToString();
            return operationsList;


        }

        


        private Dictionary<string, JObject> getNeededTyesWithContents(string sourceFolder, string destinationFolder)
        {
            string[] fileList = Directory.GetFiles(sourceFolder);
            Dictionary<string, JObject> filesMap = new Dictionary<string, JObject>();
            
            foreach (var file in fileList)
            {
                (bool isNeeded, string fileType) = getFileMatch(file);
                
                if (isNeeded)
                {
                    string stringContent = _fh.ReadFileAsString(file);
                    JObject data = JObject.Parse(stringContent);
                    filesMap.Add(fileType, data);
                    
                    // for debbugging
                    // todo use file seperator
                    Directory.CreateDirectory(destinationFolder);
                    string newFileName = destinationFolder + fileType + ".json";
                    File.Copy(file, newFileName, true);
                }
            }
            return filesMap;
        }

        private (bool, string) getFileMatch(string file)
        {
            //bool isNeeded = neededTypes.Any(s => file.Contains(s+".template"));

            foreach (string t in neededTypes)
            {
                if (file.Contains(t + ".template"))
                {
                    return (true, t);
                }
            }

            return (false, null);
        }
    }
}






























