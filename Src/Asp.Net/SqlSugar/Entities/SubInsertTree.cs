using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class GetSubInsertTree
    {
        public object Expression { get; set; }
        public List<GetSubInsertTree> ChildExpression { get; set; }
    }

    internal class SubInsertTreeExpression
    {
        public Expression Expression { get; set; }
        public List<SubInsertTreeExpression> Childs { get; set; }
    }
}
