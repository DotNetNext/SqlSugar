using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public class ObjectFuncModel: IFuncModel
    {
        public string FuncName { get; set; }
        public List<object> Parameters { get; set; }
    }
    public class ArrayFuncModel: IFuncModel
    {
        public List<object> Objects { get; set; }
    }
    
}
