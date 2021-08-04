namespace SqlSugar
{
    public interface IParameterInsertable<T>
    {
        InsertableProvider<T> Inserable { get; set; }
    }
}