namespace SqlSugar
{
    public class MySqlExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context { get; set; }
        public MySqlExpressionContext()
        {
            base.DbMehtods = new MySqlMethod();
        }

    }
    public class MySqlMethod : DefaultDbMethod, IDbMethods
    {

    }
}
