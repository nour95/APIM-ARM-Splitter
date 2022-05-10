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
        
        public Dictionary<string, JObject> GetNeededTypesWithContents(string sourceFolder, 
                        string destinationFolder = "onlyIfDebugIsTrue", bool debug = false)
        {
            // get the name of all the files inside the sourceFolder
            string[] fileList = FileHandler.GetNameOfAllFilesInDirectory(sourceFolder);
            
            // Map each file name to its content if this file name is included in the needed types
            Dictionary<string, JObject> filesMap = new Dictionary<string, JObject>();
            
            foreach (var file in fileList)
            {
                (string fileType, bool isNeeded) = GetFileMatch(file);
                
                if (isNeeded)
                {
                    // since this type of files is needed, get its content
                    string stringContent = FileHandler.ReadFileAsString(file);
                    JObject data = JObject.Parse(stringContent);
                    filesMap.Add(fileType, data);
                    
                    // for debugging
                    if (debug)
                    {
                        string newFileName = fileType + ".json";
                        string destFolder = Path.Combine(destinationFolder, "neededTypes");
                        FileHandler.CopyFile(file, destFolder, newFileName, true);
                    }
                }
            }
            return filesMap;
        }
        
        /// <summary>
        /// Check if the name of this file is the contained in the needed types (in the Globals class)
        /// and in this case return true and the matched type. otherwise return false and null
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private (string, bool) GetFileMatch(string fileName)
        {
            foreach (string type in Globals.NeededTypes)
            {
                // '-' and '.template' are just to make more sure that we are matching the correct type
                if (fileName.Contains($"-{type}.template"))
                {
                    return (type, true);
                }
            }

            return (null, false);
        }

    }
}