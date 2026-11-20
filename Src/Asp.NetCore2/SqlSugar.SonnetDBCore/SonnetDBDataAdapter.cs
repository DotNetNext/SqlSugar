using System.Data;
using System.Data.Common;

namespace SqlSugar
{
    public sealed class SonnetDBDataAdapter : DbDataAdapter, IDataAdapter
    {
        void IDataAdapter.Fill(DataSet dataSet) => Fill(dataSet);
    }
}
