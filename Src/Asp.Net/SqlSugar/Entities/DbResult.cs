using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DbResult<T>
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public string Messaage { get; set; }
        public T Data { get; set; }
    }
}
