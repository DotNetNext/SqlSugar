﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DmInsertable<T> : InsertableProvider<T> where T : class, new()
    {
       
    }
}
