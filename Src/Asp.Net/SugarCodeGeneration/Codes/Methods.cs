using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SugarCodeGeneration.Codes
{
    public class Methods
    {
        public static string GetCurrentProjectPath
        {

            get
            {
                return Environment.CurrentDirectory.Replace(@"\bin\Debug", "");
            }
        }
        public static string GetSlnPath
        {

            get
            {
                var path = Directory.GetParent(GetCurrentProjectPath).FullName;
                return path;

            }
        }

        public static void AddCsproj(string fileDirectory, string projectName)
        {
            var files = Directory.GetFiles(fileDirectory).ToList();
            var xmlPath = GetSlnPath + @"\" + projectName + @"\SugarCodeGeneration.csproj";

            var xml = File.ReadAllText(xmlPath,Encoding.UTF8);
            var firstLine = System.IO.File.ReadLines(xmlPath, Encoding.UTF8).First();
            var newXml = xml.Replace(firstLine, "").TrimStart('\r').TrimStart('\n');
            XElement xe = XElement.Parse(newXml);
        }
    }
}
