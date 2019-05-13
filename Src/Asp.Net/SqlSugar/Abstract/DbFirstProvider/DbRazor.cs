using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class RazorFirst
    {
        internal List<KeyValuePair<string,string>> ClassStringList { get;  set; }

        public static string DefaultRazorClassTemplate =
@"using System;
using System.Linq;
using System.Text;
using SqlSugar;
namespace @Model.Namespace 
{
    ///<summary>
    ///
    ///</summary>
    public partial class @Model.ClassName
    {
           public @(Model.ClassName)(){


           }
 @foreach (var item in @Model.Columns)
   {
      if(item.IsPrimarykey&&item.IsIdentity){
         @:/// <summary>
         @:/// Desc:@item.ColumnDescription
         @:/// Default:@item.DefaultValue
         @:/// Nullable:@item.IsNullable
         @:/// </summary>     
         @:[SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]      
         @:public @item.DataType @item.DbColumnName {get;set;}
         }
        else if(item.IsPrimarykey)
        {
         @:/// <summary>
         @:/// Desc:@item.ColumnDescription
         @:/// Default:@item.DefaultValue
         @:/// Nullable:@item.IsNullable
         @:/// </summary>    
         @:[SqlSugar.SugarColumn(IsPrimaryKey = true)]       
         @:public @item.DataType @item.DbColumnName {get;set;}
         } 
        else if(item.IsIdentity)
        {
         @:/// <summary>
         @:/// Desc:@item.ColumnDescription
         @:/// Default:@item.DefaultValue
         @:/// Nullable:@item.IsNullable
         @:/// </summary>       
         @:[SqlSugar.SugarColumn(IsIdentity = true)]    
         @:public @item.DataType @item.DbColumnName {get;set;}
         }
         else
         {
         @:/// <summary>
         @:/// Desc:@item.ColumnDescription
         @:/// Default:@item.DefaultValue
         @:/// Nullable:@item.IsNullable
         @:/// </summary>           
         @:public @item.DataType @item.DbColumnName {get;set;}
         }
       }

    }
}";

        public void CreateClassFile(string directoryPath)
        {
            var seChar = Path.DirectorySeparatorChar.ToString();
            if (ClassStringList.HasValue())
            {
                foreach (var item in ClassStringList)
                {
                    var filePath = directoryPath.TrimEnd('\\').TrimEnd('/') + string.Format(seChar + "{0}.cs", item.Key);
                    FileHelper.CreateFile(filePath, item.Value, Encoding.UTF8);
                }
            }
        }
        public List<KeyValuePair<string, string>> GetClassStringList()
        {
            return ClassStringList;
        }
    }
}
