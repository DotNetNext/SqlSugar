using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;

namespace OrmTest
{
    
    //[SugarTable("CustomAttributeTable")]
    public class  AttributeTable
    {

   
        //[SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
