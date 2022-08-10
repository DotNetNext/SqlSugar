using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class JoinInfoParameter
    {
       public string TableName { get; set; }
       public string ShortName{ get; set; }
       public IFuncModel Models { get; set; }
       public JoinType Type { get; set; } 
    }
}
