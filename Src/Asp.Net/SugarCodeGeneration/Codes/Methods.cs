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

        public static void AddCsproj(string classPath, string projectName)
        {
            var classDirectory = Methods.GetSlnPath + "\\" +projectName+"\\"+ classPath.TrimStart('\\');
            var files = Directory.GetFiles(classDirectory).ToList().Select(it=>classPath+"\\"+Path.GetFileName(it));
            var xmlPath = GetSlnPath + @"\" + projectName + @"\SugarCodeGeneration.csproj";

            var xml = File.ReadAllText(xmlPath,Encoding.UTF8);
            var firstLine = System.IO.File.ReadLines(xmlPath, Encoding.UTF8).First();
            var newXml = xml.Replace(firstLine, "").TrimStart('\r').TrimStart('\n');
            XDocument xe = XDocument.Load(xmlPath);
            var itemGroup=xe.Root.Elements().Where(it=>it.Name.LocalName== "ItemGroup"&&it.Elements().Any(y=>y.Name.LocalName== "Compile")).First();
            var compieList=itemGroup.Elements().ToList();
            var noAddFiles = files.Where(it => !compieList.Any(f => it.Equals(f.Attribute("Include").Value, StringComparison.CurrentCultureIgnoreCase))).ToList();
            if (noAddFiles.Any()) {
                foreach (var item in noAddFiles)
                {
                    var addItem = new XElement("Compile", new XAttribute("Include",item));
                    itemGroup.AddFirst(addItem) ;
                }
            }
            newXml = xe.ToString().Replace("xmlns=\"\"", "");
            xe = XDocument.Parse(newXml);
            xe.Save(xmlPath);
        }
    }
}
