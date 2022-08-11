using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial class SqlBuilderAccessory
    {
        protected DeleteBuilder _DeleteBuilder;

        protected InsertBuilder _InsertBuilder;

        protected QueryBuilder _QueryBuilder;

        protected SqlQueryBuilder _SqlQueryBuilder;

        protected UpdateBuilder _UpdateBuilder;
    }
}
