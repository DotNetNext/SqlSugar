using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace SqlSugar 
{
    public class QueryMethodInfo
    {
        public object QueryableObj { get; internal set; }
        public SqlSugarProvider Context { get; internal set; }

        public QueryMethodInfo Where(string sql, object parameters)
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 2, typeof(string), typeof(object));
            this.QueryableObj= method.Invoke(QueryableObj, new object[] { sql, parameters });
            return this;
        }

        public QueryMethodInfo SplitTable(DateTime begintTime,DateTime endTime)
        {
            var method = QueryableObj.GetType().GetMyMethod("SplitTable", 2, typeof(DateTime), typeof(DateTime));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] {begintTime,endTime });
            return this;
        }
        public QueryMethodInfo SplitTable()
        {
            var method = QueryableObj.GetType().GetMyMethod("SplitTable", 0);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { });
            return this;
        }

        public object ToPageList(int pageNumber,int pageSize)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageList", 2,typeof(int),typeof(int));
            var reslt = method.Invoke(QueryableObj, new object[] { pageNumber, pageSize });
            return reslt;
        }
        public object ToPageList(int pageNumber, int pageSize,ref int count)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageList", 3, typeof(int), typeof(int), typeof(int).MakeByRefType());
            var parameters = new object[] { pageNumber, pageSize, count };
            var reslt = method.Invoke(QueryableObj, parameters);
            count = parameters.Last().ObjToInt();
            return reslt;
        }
        public object ToList()
        {
            var method = QueryableObj.GetType().GetMyMethod("ToList", 0);
            var reslt = method.Invoke(QueryableObj, new object[] {   });
            return reslt;
        }

    }
}
