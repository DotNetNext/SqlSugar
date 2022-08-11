using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public enum SugarActionType
    {
        Insert=0,
        Update=1,
        Delete=2,
        Query=3,
        UnKnown = -1
    }
}
