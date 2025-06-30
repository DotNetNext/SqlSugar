using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb
{
    public class BaseCommonExpression
    {  
        protected bool IsJoinByExp(MongoNestedTranslatorContext context)
        {
            return (context?.queryBuilder is MongoDbQueryBuilder mb && !(mb.EasyJoin == null || mb.EasyJoin == true));
        }
    }
}
