using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar 
{
    public partial class PostgreSQLExpressionContext
    {
    }
    public partial class DmExpressionContext
    {
    }
    public partial class OracleExpressionContext
    {
    }
    public partial class MySqlBlukCopy<T> 
    {
        public MySqlBlukCopy(SqlSugarProvider context, ISqlBuilder builder, T[] entitys)
        {
            this.Context = context;
            this.Builder = builder;
            this.Entitys = entitys;
        }
    }
    public partial class OracleBlukCopy
    {
    }
}
