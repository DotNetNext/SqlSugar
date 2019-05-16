using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class DB2Queryable<T> : QueryableProvider<T>
    {
        public override ISugarQueryable<T> With(string withString)
        {
            return this;
        }
        protected override List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.DbColumnName).ToList();
        }
    }
    public class DB2Queryable<T, T2> : QueryableProvider<T, T2>
    {

    }
    public class DB2Queryable<T, T2, T3> : QueryableProvider<T, T2, T3>
    {

    }
    public class DB2Queryable<T, T2, T3, T4> : QueryableProvider<T, T2, T3, T4>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4, T5>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5, T6>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T, T2, T3, T4, T5, T6, T7>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {

    }
    public class DB2Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {

    }
}
