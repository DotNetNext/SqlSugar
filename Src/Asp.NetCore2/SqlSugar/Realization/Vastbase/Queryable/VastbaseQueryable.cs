namespace SqlSugar
{
    public class VastbaseV5Queryable<T> : PostgreSQLQueryable<T>
    {
        public override ISugarQueryable<T> With(string withString)
        {
            var mode = VastbaseV5Mode.Require(this.Context);
            if (mode.IsBabelfish)
                QueryBuilder.TableWithString = withString;
            return this;
        }
    }

    public class VastbaseV5Queryable<T, T2> : PostgreSQLQueryable<T, T2>
    {
        public new ISugarQueryable<T, T2> With(string withString)
        {
            var mode = VastbaseV5Mode.Require(this.Context);
            if (mode.IsBabelfish)
                QueryBuilder.TableWithString = withString;
            return this;
        }
    }

    public class VastbaseV5Queryable<T, T2, T3> : PostgreSQLQueryable<T, T2, T3>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4> : PostgreSQLQueryable<T, T2, T3, T4>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5> : PostgreSQLQueryable<T, T2, T3, T4, T5>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7, T8> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7, T8>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
    }

    public class VastbaseV5Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : PostgreSQLQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
    }
}
