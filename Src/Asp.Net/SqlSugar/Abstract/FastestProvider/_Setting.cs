using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class FastestProvider<T> : IFastest<T> where T : class, new()
    {
        public IFastest<T> AS(string tableName)
        {
            this.AsName = tableName;
            return this;
        }
        public IFastest<T> PageSize(int size)
        {
            this.Size = size;
            return this;
        }
    }
}
