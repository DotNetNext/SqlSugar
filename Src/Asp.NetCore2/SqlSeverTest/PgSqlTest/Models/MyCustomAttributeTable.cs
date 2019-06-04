using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [Table("CustomAttributeTable")]
    //[SugarTable("CustomAttributeTable")]
    public class MyCustomAttributeTable
    {

        [Key]
        //[SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
