using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;

namespace OrmTest
{
    
    //[SugarTable("CustomAttributeTable")]
    public class MyCustomAttributeTable
    {

      
        //[SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
