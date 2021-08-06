using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IParameterInsertable<T>
    {
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
    }
}