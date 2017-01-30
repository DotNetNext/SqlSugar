using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class DefaultDbMethod : IDbMethods
    {
        public virtual string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            if (parameter.IsMember)
            {
                return string.Format("( {0}='' OR {0} IS NULL )", parameter.Value);
            }
            else
            {
                return null;
            }

        }
    }
}
