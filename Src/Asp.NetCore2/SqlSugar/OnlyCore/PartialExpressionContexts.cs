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
        internal SqlSugarProvider Context { get; set; }
        internal ISqlBuilder Builder { get; set; }
        internal T[] Entitys { get; set; }
        internal string Chara { get; set; }

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
