using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    internal class JsonCommonProvider
    {
        public JsonCommonProvider(ISqlSugarClient context)
        {
            //this.context = context;
            this.sqlBuilder = InstanceFactory.GetSqlbuilder(context.CurrentConnectionConfig);
            if (context is SqlSugarProvider)
            {
                this.sqlBuilder.Context = context as SqlSugarProvider;
            }
            else if (context is SqlSugarScopeProvider)
            {
                this.sqlBuilder.Context = (context as SqlSugarScopeProvider).conn;
            }
            else if(context is SqlSugarScope)
            {
                this.sqlBuilder.Context = (context as SqlSugarScope).GetConnection(context.CurrentConnectionConfig.ConfigId);
            }
            else
            {
                this.sqlBuilder.Context = (context as SqlSugarClient).Context;
            }
        }
        //public ISqlSugarClient context { get; set; }
        public ISqlBuilder sqlBuilder { get; set; }
        public int ParameterIndex { get { return ((SqlBuilderProvider)sqlBuilder)?.GetParameterNameIndex??0; } }
        public JsonTableNameInfo GetTableName(JToken item)
        {
            JsonTableNameInfo jsonTableNameInfo = new JsonTableNameInfo();
            if (item.First().Type == JTokenType.Array && item.First.Count() == 2)
            {
                var tableName = item.First()[0].ObjToString();
                var shortName = item.First()[1].ObjToString();
                jsonTableNameInfo.ShortName = shortName;
                jsonTableNameInfo.TableName = tableName;

            }
            else
            {
                var value = item.First().ToString();
                jsonTableNameInfo.TableName = value;
            }
            return jsonTableNameInfo;
        }
        public KeyValuePair<string, SugarParameter[]> GetWhere(string item, SqlSugarProvider context)
        {

            if (!IsConditionalModel(item))
            {
                var obj = context.Utilities.JsonToSqlFuncModels(item);
                var sqlobj = sqlBuilder.FuncModelToSql(obj);
                return sqlobj;
            }
            else
            {
                var obj = context.Utilities.JsonToConditionalModels(item);
                var sqlObj = sqlBuilder.ConditionalModelToSql(obj, 0);
                return sqlObj;
            }
        }
        public KeyValuePair<string,SugarParameter[]> GetWhere(JToken item,SqlSugarProvider context)
        {
            var value = item.First().ToString();
            Check.ExceptionEasy(item.First().Type != JTokenType.Array, "Where format error " + item, "Where格式错误" + item);
            if (!IsConditionalModel(value))
            {
                var obj = context.Utilities.JsonToSqlFuncModels(value);
                var sqlobj = sqlBuilder.FuncModelToSql(obj);
                return sqlobj;
            }
            else
            {
                var obj = context.Utilities.JsonToConditionalModels(value);
                var sqlObj = sqlBuilder.ConditionalModelToSql(obj, 0);
                return sqlObj;
            }
        }

        private static bool IsConditionalModel(string value)
        {
            return value.ToLower().Contains("fieldname");
        }
    }
}
