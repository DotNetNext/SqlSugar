using System;
using System.Collections.Generic;

namespace SqlSugar
{
    public class VastbaseV5DbBind : PostgreSQLDbBind
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public override string GetPropertyTypeName(string dbTypeName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbBind.GetPropertyTypeName(dbTypeName);
            return base.GetPropertyTypeName(dbTypeName);
        }

        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var mode = Mode;
                if (mode != null && mode.IsBabelfish) return mode.MSSQLDbBind.MappingTypes;
                return base.MappingTypes;
            }
        }

        public override string GetDbTypeName(string csharpTypeName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbBind.GetDbTypeName(csharpTypeName);
            return base.GetDbTypeName(csharpTypeName);
        }

        public override List<T> DataReaderToList<T>(Type type, System.Data.IDataReader dataReader)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                mode.MSSQLDbBind.QueryBuilder = this.QueryBuilder;
                return mode.MSSQLDbBind.DataReaderToList<T>(type, dataReader);
            }
            return base.DataReaderToList<T>(type, dataReader);
        }
    }
}