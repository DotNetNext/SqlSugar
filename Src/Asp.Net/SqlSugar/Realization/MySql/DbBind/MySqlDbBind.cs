using System;
using System.Collections.Generic;

namespace SqlSugar
{
    public class MySqlDbBind : DbBindProvider
    {
        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                return new List<KeyValuePair<string, CSharpDataType>>()
                {
                    new KeyValuePair<string, CSharpDataType>("tinyint",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("mediumint",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("int",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                };
            }
        }
    }
}
