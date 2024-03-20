using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class ISqlSugarClientExtensions
    { 
        public static QuestDbRestAPI RestApi(this ISqlSugarClient db) 
        {
            return new QuestDbRestAPI(db);
        }
    }    
}
