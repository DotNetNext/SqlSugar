﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class ExpressionContextCase
    {
        public bool IsDateString { get; set; }
        public bool HasWhere { get; set; }
        public int Num { get; set; } = 1;
    }
}
