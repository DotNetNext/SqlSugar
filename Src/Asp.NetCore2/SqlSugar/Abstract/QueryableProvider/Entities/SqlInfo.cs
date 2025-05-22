﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{

    internal class SqlInfo
    {
        public bool IsSelectNav { get; set; }
        public int? Take { get; set; }
        public int? Skip { get; set; }
        public string WhereString { get; set; }
        public string OrderByString { get; set; }
        public string SelectString { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public List<MappingFieldsExpression> MappingExpressions { get; set; }
        public string TableShortName { get; set; }
        public Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> SplitTable { get; set; }
    }
}
