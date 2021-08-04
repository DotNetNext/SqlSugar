using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ParameterInsertable<T> : IParameterInsertable<T> where T:class,new()
    {
        public InsertableProvider<T> Inserable { get; set; }
        public int ExecuteCommand() 
        {
            return 0;
        }
    }
}
