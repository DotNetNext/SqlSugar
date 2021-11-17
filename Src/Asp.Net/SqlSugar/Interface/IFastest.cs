using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public interface IFastest<T>
    {
        int BulkCopy(List<T> datas);
         Task<int> BulkCopyAsync(List<T> datas);
    }
}
