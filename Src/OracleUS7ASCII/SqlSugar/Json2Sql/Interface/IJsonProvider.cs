using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{

    public interface IJsonProvider<T>
    {
        List<SqlObjectResult> ToSqlList();
        SqlObjectResult ToSql();
        List<string> ToSqlString();
        T ToResult();
    }
}
