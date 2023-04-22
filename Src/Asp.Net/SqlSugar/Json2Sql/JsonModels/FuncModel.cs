using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ObjectFuncModel: IFuncModel
    {
        public string FuncName { get; set; }
        public List<object> Parameters { get; set; }

        public static ObjectFuncModel Create(string FuncName, params object[] Parameters) 
        {
            return new ObjectFuncModel() { FuncName = FuncName, Parameters = Parameters?.ToList() };
        }
    }
    public class ArrayFuncModel: IFuncModel
    {
        public List<object> Objects { get; set; }
    }
    
}
