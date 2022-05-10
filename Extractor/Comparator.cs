﻿using System.Collections.Generic;
using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class Comparator
    {
        /// <summary>
        /// Will compare the new file with old files. Return an object of class summery that
        /// include 4 types of lists.
        /// </summary>
        /// <param name="oldFile"></param>
        /// <param name="newFile"></param>
        /// <returns>Retrun an object of class summery that have 4 types of lists:
        /// - All: Will include all the resources that exist in the new file. Can be used for debugging purposes.
        /// - Matched: Will include all the resources that exist in both new and old file, even if there is some inner
        ///       differences between the 2 resources. Can be used for debugging purposes.
        /// - UnMatched: Will include only the new resources that do not exist at all in the old file.
        /// - Inner difference: This will describe the inner differences between the old resource and the new one. 
        ///        i.e. the resource exist in both new and old file but there is some differences between the
        ///        content of these resources
        /// </returns>
        public Summery Compare(JObject oldFile, JObject newFile)
        {
            List<JObject> all = new List<JObject>();       // can be needed for debugging purposes. 
            List<JObject> matched = new List<JObject>();   // can be needed for debugging purposes. 
            
            // This variable will have onl the new resources that don't exist in the old file.
            List<JObject> unMatched = new List<JObject>();
            
            // This will describe the inner differences between the old resource and the new resource. 
            // i.e. the resource exist in both new and old file but they have some differences
            List<Operations> innerDifferences = new List<Operations>();

            // Go through all resources in the nre file
            foreach (JObject resourceInNew in newFile["resources"])
            {
                all.Add(resourceInNew);
                
                string newResourceName = resourceInNew["name"].ToString();
                string newResourceType = resourceInNew["type"].ToString();
                
                var found = false;

                foreach (JObject resourceInOld in oldFile["resources"])
                {
                    string oldResourceName = resourceInOld["name"].ToString();
                    string oldResourceType = resourceInOld["type"].ToString();
                    
                    // if the two resources has the same name and type, then this is a match
                    // todo may replace this later to have a real comparison between 2 json object
                    if (oldResourceName == newResourceName && oldResourceType == newResourceType)
                    {
                        found = true;
                        matched.Add(resourceInNew);

                        // now try to find the inner differences between 2 resources
                        List<OneOperation> comparedResult = compareContents(resourceInOld, resourceInNew);
                        
                        // if there is at least one difference then add this difference ti innerDifference list
                        if (comparedResult != null && comparedResult.Count >= 1)
                        {
                            Operations operationsList = new Operations(newResourceName, newResourceType, comparedResult,
                                resourceInNew, resourceInOld);
                            innerDifferences.Add(operationsList);
                        }

                        break;
                    }
                }
                // else if there is no resource in the old file that have this same name as the new resource name
                // and the same type of the new resource type --> Then, this is an unmatch.
                if(!found)
                    unMatched.Add(resourceInNew);
            }
            
            return new Summery(all, matched, unMatched, innerDifferences);



        }

        /// <summary>
        /// This will use JsonDiffPatch library to compare the content of the 2 resources.
        /// </summary>
        /// <param name="resourceInOld"></param>
        /// <param name="resourceInNew"></param>
        /// <returns>Return a list of all different operations between the 2 resources like add,
        /// remove, replace.</returns>
        private List<OneOperation> compareContents(JObject resourceInOld, JObject resourceInNew)
        {
            var jdp = new JsonDiffPatch();

            JToken patch = jdp.Diff(resourceInOld, resourceInNew);
            // var output = jdp.Patch(resourceInNew, patch);
            var formatter = new JsonDeltaFormatter();
            var operations = formatter.Format(patch);
            
            // convert the library (JsonDiffPatch) own operation class to my own operation class to have
            // a better control of the results
            var operationString = JsonConvert.SerializeObject(operations,Formatting.Indented);
            var operationsList = JsonConvert.DeserializeObject<List<OneOperation>>(operationString);
            
            return operationsList;
        }

        
    }
}