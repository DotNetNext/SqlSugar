using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubFromTable : ISubOperation
    {
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Sort
        {
            get
            {
                return 300;
            }
        }
        public string GetValue(ExpressionContext context, Expression expression = null)
        {
            return context.GetTranslationTableName(expression.Type.Name, true);
        }
    }
}
