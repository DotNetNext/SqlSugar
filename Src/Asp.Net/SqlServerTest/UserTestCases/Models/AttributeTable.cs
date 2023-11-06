using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace OrmTest
{
    [Table(Name = "MyAttributeTable")]
    //[SugarTable("CustomAttributeTable")]
    public class  AttributeTable
    {

        [Key]
        //[SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
