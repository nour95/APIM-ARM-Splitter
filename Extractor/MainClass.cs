using System;

namespace Extractor
{
    public class MainClass
    {
        
        
        
        private static void Main(string[] args)
        {
            string apiPath =
                @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure2\gk-api-dev-serviceagreements-api.template.json";
            string folderPath = @"C:\Users\NourAl-HudaAl-Majni\Desktop\nour\ibiz\GK\gitReopos\APIs\API-ServiceAgreements\Azure3";
            
            
            FileHandler fh = new FileHandler(apiPath, folderPath);
            
            string data = fh.ReadFile();
            fh.SplitJson(data);
            
            fh.PrintAllFiles();
            
            Console.WriteLine("Hello World!");
            
        }
    }
}