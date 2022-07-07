using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        private void DeleteManyToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {

        }

    }
}
