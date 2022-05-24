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



        public MainClass()
        {
            // this._allResources = new Dictionary<ResourceTypes, List<JObject>>();

            this._fh = new FileHandler();
            this._splitter = new Splitter();
            this._comparator = new Comparator();
        }

        private static void Main(string[] args)
        {
            //TODO make one path that point to the folder only
            // string sourceFolder = Path.Combine(mainPath, "Nour", "d365");
            // string destinationFolder = Path.Combine(mainPath, "Nour", "results");
            //
            // string oldSourceFolder = Path.Combine(mainPath, "APIM", "D365");

            MainClass mc = new MainClass();

            mc.Run();
        }

        private void Run()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Reading the files .....");
            
            Dictionary<string, JObject> oldFilesMap = _splitter.GetNeededTypesWithContents(Globals.OldSrcFolder);
            Dictionary<string, JObject> newFilesMap =
                _splitter.GetNeededTypesWithContents(Globals.SrcFolder, Globals.DestFolder, true);

            //List<JObject> all; 
            // List<JObject> matched;
            // List<JObject> unMatched; 
            // List<Operations> innerDiff;
            
            //string type = Globals.NeededTypes[3];
            foreach (var type in Globals.NeededTypes)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Compare the '{type}' files .....");

                Summery s = _comparator.Compare(oldFilesMap[type], newFilesMap[type]);

                var (all, matched) = (s.All, s.Matched);
                var (unMatched, innerDiff) = (s.UnMatched, s.InnerDifferences);
                int taskDiffErrorCounter = s.TaskDiffErrorCounter;
                
                //var (all, matched, unMatched, innerDiff ) = (s.All, s.Matched, s.UnMatched, s.InnerDifferences);

                
                // print the new resources and the inner differences
                FileHandler.PrintJsonInFile(unMatched, Path.Join(Globals.DestFolder, "newResources"), type);
                FileHandler.PrintInnerDifferencesInFile(innerDiff, Path.Join(Globals.DestFolder, "innerDifferences"), type);
                
                // Console.WriteLine("-----------------------------------");
                // Console.WriteLine($"Done comparing the '{type}' files ");
                Console.WriteLine("************************************");
                Console.WriteLine($"Number of All new resources = '{all.Count}'");
                Console.WriteLine($"Of them there are '{unMatched.Count}' completely new resources");
                Console.WriteLine($"              and '{innerDiff.Count}' resources that have some inner differences");
                Console.WriteLine($"[For debugging] The Number of resources that exist in both new and old file = '{matched.Count}'");
                Console.WriteLine($"[For debugging] Through that the code has found '{taskDiffErrorCounter}' taskDiffErrors");
                Console.WriteLine("************************************");

            }

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Done Comparing and creating new files!");
            
            // todo do something if there is no new resources
        }

        
        
        
    }
}