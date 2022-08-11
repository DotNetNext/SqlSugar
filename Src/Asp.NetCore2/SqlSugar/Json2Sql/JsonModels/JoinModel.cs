using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{

    public class JoinModel
    {
        public string TableName { get; set; }
        public string ShortName { get; set; }
        public ObjectFuncModel OnWhereList { get; set; }
        public JoinType JoinType { get; set; }
    }
}
