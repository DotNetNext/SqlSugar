namespace SqlSugar.IoTDB
{
    public sealed class IoTDBInsertBuilder : InsertBuilder
    {
        public override string SqlTemplate => "INSERT INTO {0} ({1}) VALUES ({2})";
        public override string SqlTemplateBatch => "INSERT INTO {0} ({1})";
        public override string SqlTemplateBatchUnion => " VALUES ";
        public override string SqlTemplateBatchSelect => " {0} ";
    }
}
