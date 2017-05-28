using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum InitKeyType
    {
        /// <summary>
        /// Init primary key and identity key from the system table 
        /// </summary>
        SystemTable = 0,
        /// <summary>
        /// Init primary key and identity key from the attribute 
        /// </summary>
        Attribute = 1
    }
}
