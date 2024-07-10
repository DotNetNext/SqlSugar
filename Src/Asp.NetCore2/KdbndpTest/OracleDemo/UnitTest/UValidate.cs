using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbndpTest.OracleDemo.UnitTest
{
    public class UValidate
    {
        public static void Check(object a, object b, object name)
        {
            if (a?.ToString()?.Replace(" ", "").Trim().ToLower() != b?.ToString().Replace(" ", "")?.Trim().ToLower())
            {
                throw new Exception(name + " error");
            }
        }
    }
}
