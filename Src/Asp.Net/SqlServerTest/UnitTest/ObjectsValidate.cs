using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class ObjectsValidate
    {
        public static void Check(object a ,object b,object name)
        {
            if (a?.ToString() != b?.ToString())
            {
                new Exception(name+" error");
            }
        }
    }
}
