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
        private static string mainPath =
            @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\A1\gitRepos\shared\Shared.APIM";
        // private readonly string[] neededTypes = new[]{"apiTags", "apiVersionSets", "tags", "globalServicePolicy" , "api"};


        private readonly FileHandler _fh;
        private readonly Splitter _splitter;
        private readonly Comparator _comparator;

        //TODO find a solution to them later
        private readonly string srcFolder;
        private readonly string destFolder;
        private readonly string oldSourceFolder;


        public MainClass(string srcFolder, string destFolder, string oldSourceFolder)
        {
            // this._allResources = new Dictionary<ResourceTypes, List<JObject>>();

            this._fh = new FileHandler();
            this._splitter = new Splitter();
            this._comparator = new Comparator();

            this.srcFolder = srcFolder;
            this.destFolder = destFolder;
            this.oldSourceFolder = oldSourceFolder;
        }

        private static void Main(string[] args)
        {
            //TODO make one path that point to the folder only
            string sourceFolder = Path.Combine(mainPath, "Nour", "d365");
            string destinationFolder = Path.Combine(mainPath, "Nour", "results");

            string oldSourceFolder = Path.Combine(mainPath, "APIM", "D365");

            MainClass mc = new MainClass(sourceFolder, destinationFolder, oldSourceFolder);

            mc.Run();
        }

        private void Run()
        {
            Dictionary<string, JObject> oldFilesMap = _splitter.GetNeededTypesWithContents(oldSourceFolder);
            Dictionary<string, JObject> newFilesMap =
                _splitter.GetNeededTypesWithContents(srcFolder, destFolder, true);

            // foreach (var type in Globals.NeededTypes)
            string type = Globals.NeededTypes[0];
            {
                Summery s = _comparator.Compare(oldFilesMap[type], newFilesMap[type]);

                // (_, _, var unMatched, var innerDiff) = (s.All, s.Matched, s.UnMatched, s.InnerDifferences);
                (var unMatched, var innerDiff) = (s.UnMatched, s.InnerDifferences);
                
                // ptiny the new resources and the inner differences
                FileHandler.PrintJsonInFile(unMatched, Path.Join(destFolder, "newResources"), type);
                FileHandler.PrintInnerDifferencesInFile(innerDiff, Path.Join(destFolder, "innerDifferences"), type);
            }

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Done Comparing and creating new files!");
        }

        
        
        
        
        private List<OneOperation> compareContents(JObject resourceInOld, JObject resourceInNew)
        {
            var jdp = new JsonDiffPatch();
            try
            {
                JToken patch = jdp.Diff(resourceInOld, resourceInNew);
                var output = jdp.Patch(resourceInNew, patch);
                var formatter = new JsonDeltaFormatter();
                var operations = formatter.Format(patch);
                
                var operationString = JsonConvert.SerializeObject(operations, Formatting.Indented);
                var operationsList = JsonConvert.DeserializeObject<List<OneOperation>>(operationString);
                
                return operationsList;

            }
            catch (Exception e)
            {
                Console.WriteLine( JsonConvert.SerializeObject(resourceInOld, Formatting.Indented));
                Console.WriteLine("*-*-*-*-*-////*****/*--------");
                Console.WriteLine( JsonConvert.SerializeObject(resourceInNew, Formatting.Indented));

                throw;
            }

            
        }
    }
}