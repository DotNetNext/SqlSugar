using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.TDengine
{
    public class STable
    {
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public string TagsTypeId { get; set; }
        public static List<ColumnTagInfo> Tags = null;
    }
    public class ColumnTagInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
