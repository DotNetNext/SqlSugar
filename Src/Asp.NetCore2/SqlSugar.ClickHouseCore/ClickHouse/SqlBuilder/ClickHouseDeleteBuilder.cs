namespace SqlSugar.ClickHouse
{
    public class ClickHouseDeleteBuilder : DeleteBuilder
    {
     
        public override string SqlTemplate => "ALTER TABLE {0} DELETE {1}";
    }
}
