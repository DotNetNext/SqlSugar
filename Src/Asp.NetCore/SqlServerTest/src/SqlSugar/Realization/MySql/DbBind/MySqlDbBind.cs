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
                throw new NotImplementedException();
            }
        }
    }
}
