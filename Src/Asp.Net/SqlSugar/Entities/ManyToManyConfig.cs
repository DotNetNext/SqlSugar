using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ManyToMany
    {
        public static ManyToMany Config<AClass,AField,BClass,BField>(AClass aClass,AField aField,BClass bClass,BField bField) 
            where AClass:class
            where AField : struct
            where BClass:class
            where BField:struct
        {
            return null;
        }
    }
}
