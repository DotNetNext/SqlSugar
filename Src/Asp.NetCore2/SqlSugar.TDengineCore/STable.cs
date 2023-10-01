using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.TDengine
{
    public class STable
    {
        [SugarColumn(IsOnlyIgnoreInsert =true ,IsOnlyIgnoreUpdate =true)]
        public string TagsTypeId { get; set; } 
    }
}
