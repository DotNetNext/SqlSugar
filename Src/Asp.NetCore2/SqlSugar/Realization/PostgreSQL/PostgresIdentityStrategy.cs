namespace SqlSugar.Realization.PostgreSQL
{
    public enum PostgresIdentityStrategy
    {
        /// <summary>
        /// NEXTVAL() function
        /// </summary>
        Serial = 1,
        /// <summary>
        /// 自增长，PGSQL10+版本最佳实践
        /// </summary>
        Identity = 2
    }
}
