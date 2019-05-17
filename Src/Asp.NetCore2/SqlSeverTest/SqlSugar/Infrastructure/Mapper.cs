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
            List<KeyValuePair<string, object>> selectItems = new List<KeyValuePair<string, object>>();
            foreach (var viewColumn in veiwModel.Columns)
            {
                var exParsmeters = exp.Parameters.Select(it => new { shortName = it.Name, type = it.Type }).ToList();
                foreach (var expPars in exParsmeters)
                {
                    var columns = _context.EntityMaintenance.GetEntityInfo(expPars.type).Columns.Where(it => it.IsIgnore == false);
                    var joinModelDbColumns = columns.Select(it =>
                    new { asName = it.PropertyName, dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelProperties = columns.Select(it =>
                    new { asName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelDbColumnsWithType = columns.Select(it =>
                    new { asName = (expPars.type.Name + it.PropertyName), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelPropertiesWithTye = columns.Select(it =>
                    new { asName = (expPars.type.Name + _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type)), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelDbColumns_WithType = columns.Select(it =>
                    new { asName = (expPars.type.Name +"_"+ it.PropertyName), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelProperties_WithType = columns.Select(it =>
                    new { asName = (expPars.type.Name +"_"+ _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type)), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelDbColumnsWithSN = columns.Select(it =>
                    new { asName = (expPars.shortName + it.PropertyName), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();
                    var joinModelPropertiesWithSN = columns.Select(it =>
                    new { asName = (expPars.shortName + _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type)), dbName = _context.EntityMaintenance.GetDbColumnName(it.PropertyName, expPars.type) }).ToList();

                    if (joinModelDbColumns.Any(it => it.asName.Equals(viewColumn.PropertyName,StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelDbColumns.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName,value));
                        break;
                    }
                    if (joinModelProperties.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelProperties.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }
                    if (joinModelDbColumnsWithType.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelDbColumnsWithType.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }
                    if (joinModelPropertiesWithTye.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelPropertiesWithTye.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }

                    if (joinModelDbColumns_WithType.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelDbColumns_WithType.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }
                    if (joinModelProperties_WithType.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelProperties_WithType.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }
                    if (joinModelDbColumnsWithSN.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelDbColumnsWithSN.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
                    }
                    if (joinModelPropertiesWithSN.Any(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var value = joinModelPropertiesWithSN.First(it => it.asName.Equals(viewColumn.PropertyName, StringComparison.CurrentCultureIgnoreCase));
                        if (viewColumn.PropertyName.Equals(value.asName, StringComparison.CurrentCultureIgnoreCase))
                            selectItems.Add(new KeyValuePair<string, object>(expPars.shortName, value));
                        break;
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
