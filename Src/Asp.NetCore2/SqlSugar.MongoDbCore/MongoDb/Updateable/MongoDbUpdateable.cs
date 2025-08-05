using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace SqlSugar.MongoDb
{
    public  class MongoDbUpdateable<T>:UpdateableProvider<T> where T:class,new()
    {
        public override IUpdateable<T> SetColumns(Expression<Func<T, bool>> columns)
        {
            ThrowUpdateByObject(); 
            var binaryExp = columns.Body as BinaryExpression;
            Check.Exception(!binaryExp.NodeType.IsInValues(ExpressionType.Equal), "No support {0}", columns.ToString());
            Check.Exception(!(binaryExp.Left is MemberExpression) && !(binaryExp.Left is UnaryExpression), "No support {0}", columns.ToString());
            Check.Exception(ExpressionTool.IsConstExpression(binaryExp.Left as MemberExpression), "No support {0}", columns.ToString());
          
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.WhereSingle).GetResultString().Replace(")", " )").Replace("(", "( ").Trim().TrimStart('(').TrimEnd(')').Replace("= =", "=");
            var bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(expResult);
            foreach (var element in bson)
            {
                string key = element.Name;
                BsonValue value = element.Value;
                this.UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(key, expResult));
            } 
            return this;
        }

        public override IUpdateable<T> SetColumns(Expression<Func<T, T>> columns)
        {
            ThrowUpdateByObject();
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update)
                .GetResultString();
            this.UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>("$set", expResult));
            return this;
        }
        public override IUpdateable<T> SetColumns(Expression<Func<T, T>> columns, bool appendColumnsByDataFilter)
        {
            ThrowUpdateByObject();
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update)
                .GetResultString();
            if (appendColumnsByDataFilter)
            {
                var newData = new T() { };
                SqlSugar.UtilMethods.ClearPublicProperties(newData, this.EntityInfo);
                var data = ((UpdateableProvider<T>)this.Context.Updateable(newData)).UpdateObjs.First();
                foreach (var item in this.EntityInfo.Columns.Where(it => !it.IsPrimarykey && !it.IsIgnore && !it.IsOnlyIgnoreUpdate))
                {
                    var value = item.PropertyInfo.GetValue(data);
                    if (value != null && !value.Equals(""))
                    {
                        if (!value.Equals(SqlSugar.UtilMethods.GetDefaultValue(item.UnderType)))
                        {
                            var pName = this.SqlBuilder.SqlParameterKeyWord + item.PropertyName + 1000;
                            var p = new SugarParameter(pName, value);
                            this.UpdateBuilder.Parameters.Add(p);
                            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(item.DbColumnName), SqlBuilder.GetTranslationColumnName(item.DbColumnName) + "=" + pName));
                        }
                    }
                }
            }
            this.UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>("$set", expResult));
            return this;
        }
    }
}
