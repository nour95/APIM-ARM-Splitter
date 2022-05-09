using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    public class Extractor
    {

        public List<JObject> GetOperationByName(List<JObject> operationList, string operationName)
        {
            string name;
            List<JObject> list = new List<JObject>();
            
            foreach (var operation in operationList)
            {
                name = operation["name"].Value<string>(); // todo not sure if contain is better than endWith??
                if (name != null && name.Contains(operationName))
                {
                    list.Add(operation);
                }
            }
            
            // otherwise
            Console.WriteLine($"No operation found in the file with name {operationList}");
            return list;
        }



    }
}