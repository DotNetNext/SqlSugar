using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar 
{
    public partial class InsertNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        private void InsertManyToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
         
        }
    }
}
