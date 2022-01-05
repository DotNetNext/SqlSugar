using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SugarMapper
    {
        private ISqlSugarClient _context;
        public SugarMapper(ISqlSugarClient context)
        {
            _context = context;
        }

        public string GetSelectValue<TResult>(QueryBuilder queryBuilder)
        {
            string result = string.Empty;
            var veiwModel = _context.EntityMaintenance.GetEntityInfo<TResult>();
            var exp = (queryBuilder.JoinExpression as LambdaExpression);
            List<KeyValuePair<string, JoinMapper>> selectItems = new List<KeyValuePair<string, JoinMapper>>();
            var exParsmeters = exp.Parameters.Select(it => new { shortName = it.Name, type = it.Type }).ToList();
            foreach (var viewColumns in veiwModel.Columns)
            {
                var isbreak = false;
                foreach (var expPars in exParsmeters)
                 {
                    if (isbreak)
                    {
                        break;
                    }
                    var entityInfo = _context.EntityMaintenance.GetEntityInfo(expPars.type);
                    var columns = entityInfo.Columns.Where(it => it.IsIgnore == false);
                    var list = columns.Select(it => {
                        var array = new string[]
                        {
                            it.PropertyName,
                            it.DbColumnName,

                            expPars.type.Name+it.PropertyName,
                            expPars.type.Name+it.DbColumnName,

                            expPars.type.Name+"_"+it.PropertyName,
                            expPars.type.Name+"_"+it.DbColumnName,

                            expPars.shortName+it.PropertyName,
                            expPars.shortName+it.DbColumnName,
                         };
                        return new { it, array };
                    }).ToList();
                    var columnInfo= list.FirstOrDefault(y => y.array.Select(z=>z.ToLower()).Contains(viewColumns.PropertyName.ToLower()));
                    if (columnInfo != null)
                    {
                        JoinMapper joinMapper = new JoinMapper()
                        {
                             AsName=viewColumns.PropertyName,
                             DbName=columnInfo.it.DbColumnName
                        };
                        selectItems.Add(new KeyValuePair<string, JoinMapper>(expPars.shortName,joinMapper));
                        isbreak = true;
                    }
                }
            }
            result = queryBuilder.GetSelectByItems(selectItems);
            if (_context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
            {
                result = result.ToLower();
            }
            return result;
        }
    }
}
