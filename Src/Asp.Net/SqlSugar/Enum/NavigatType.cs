using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public enum NavigateType
    {
        OneToOne=1,
        OneToMany=2,
        ManyToOne=3,
        ManyToMany=4, 
        Dynamic=5
    }
}
