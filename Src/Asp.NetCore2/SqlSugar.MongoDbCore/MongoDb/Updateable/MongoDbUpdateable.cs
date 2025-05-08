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
    }
}
