using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class ReportableProvider<T> : IReportable<T>
    {
        public SqlSugarProvider Context { get; set; }
        private List<T> datas = new List<T>();
        private List<DateTime> dates = new List<DateTime>();
        private bool isDates = false;
        internal QueryBuilder queryBuilder;
        internal InsertBuilder formatBuilder { get; set; }

        public ReportableProvider(T data)
        {
            datas.Add(data);
            Init();
        }

        public ReportableProvider(List<T> list)
        {
            datas = list;
            Init();
        }

        //public IReportable<T> MakeUp(Func<T, object> auto)
        //{
        //    throw new NotImplementedException();
        //}

        public ISugarQueryable<T> ToQueryable()
        {
            StringBuilder sb = new StringBuilder();
            if (datas.Any())
            {
                if (isDates)
                {
                    var da = this.dates;
                    Each(sb, da);
                }
                else
                {
                    var da = this.datas;
                    Each(sb, da);
                }
            }
            else 
            {
                if (typeof(T).IsClass())
                {
               
                    var result = (T)Activator.CreateInstance(typeof(T), true);
                    datas.Add(result);
                    ClassMethod(result, sb, true);
                }
                else 
                {
                    sb.Append("SELECT  NULL as ColumnName ");
                    sb.Append(GetNextSql);
                }
            }
            return this.Context.SqlQueryable<object>(sb.ToString()).Select<T>();
        }
        public ISugarQueryable<SingleColumnEntity<Y>> ToQueryable<Y>()
        {
            return ToQueryable().Select<SingleColumnEntity<Y>>();
        }

        private void Each<Y>(StringBuilder sb, List<Y> list)
        {
            int i = 0;
            foreach (var item in list)
            {
                ++i;
                var isLast = i == list.Count ;
                var isClass = typeof(T).IsClass();
                if (isClass)
                {
                    ClassMethod(item, sb, isLast);
                }
                else
                {
                    NoClassMethod(item, sb, isLast);
                }
            }
        }

        private void ClassMethod<Y>(Y data, StringBuilder sb,bool isLast)
        {
            var columns = new StringBuilder();
            var entity=this.Context.EntityMaintenance.GetEntityInfo<T>();
            columns.Append(string.Join(",",entity.Columns.Where(it=>it.IsIgnore==false).Select(it=>GetSelect(it,data))));
            columns.Append(",null as NoCacheColumn");
            sb.AppendLine(" SELECT " + columns.ToString());
            sb.Append(GetNextSql);
            if (!isLast)
            {
                sb.AppendLine(" UNION ALL ");
            }
        }

        private object GetSelect<Y>(EntityColumnInfo it,Y  data)
        {

            return string.Format(" {0} AS {1} ", FormatValue(it.PropertyInfo.GetValue(data,null)),it.PropertyName);
        }

        private void NoClassMethod<Y>(Y data, StringBuilder sb,bool isLast)
        {
            sb.AppendLine(" SELECT "+  FormatValue(data));
            sb.Append(" AS ColumnName ");
            sb.Append(GetNextSql);
            if (!isLast)
            {
                sb.AppendLine(" UNION ALL ");
            }
        }
        public string GetNextSql
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                {
                    return " from dual ";
                }
                else 
                {
                    return null;
                }
            }
        }
        private void Init()
        {
            if (datas.Count == 1) 
            {
               var data=datas.First();
                isDates = data is ReportableDateType;
                if (data is ReportableDateType) 
                {
                    var type = UtilMethods.ChangeType2(data, typeof(ReportableDateType));
                    switch (type)
                    {
                        case ReportableDateType.MonthsInLast1years:
                            dates.AddRange(GetMonths(1));
                            break;
                        case ReportableDateType.MonthsInLast3years:
                            dates.AddRange(GetMonths(3));
                            break;
                        case ReportableDateType.MonthsInLast10years:
                            dates.AddRange(GetMonths(10));
                            break;
                        case ReportableDateType.years1:
                            dates.AddRange(GetYears(1));
                            break;
                        case ReportableDateType.years3:
                            dates.AddRange(GetYears(3));
                            break;
                        case ReportableDateType.years10:
                            dates.AddRange(GetYears(10));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private List<DateTime> GetYears(int v)
        {
            List<DateTime> result = new List<DateTime>(); 
            for (int i = 0; i < v; i++)
            {
                var year= (DateTime.Now.AddYears(i * -1).Year+"-01"+"-01").ObjToDate();
                result.Add(year); 
            }
            return result;
        }
        private List<DateTime> GetMonths(int v)
        {
            List<DateTime> result = new List<DateTime>();
            var years = GetYears(v);
            foreach (var item in years)
            {
                for (int i = 0; i < 12; i++)
                {
                    result.Add(item.AddMonths(i));
                }
            }
            return result;
        }
        private object FormatValue(object value)
        {
            if (value == null)
                return "null";
            var type =UtilMethods.GetUnderType(value.GetType());
            if (type.IsIn(typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort)))
            {
                return value;
            }
            else if (type.IsIn(typeof(decimal), typeof(double)))
            {
                Expression<Func<SingleColumnEntity<object>, object>> exp = it => Convert.ToDecimal(it.ColumnName);
                var result = queryBuilder.LambdaExpressions.DbMehtods.ToDecimal(new MethodCallExpressionModel() {
                 Args=new List<MethodCallExpressionArgs>() { 
                  new MethodCallExpressionArgs(){ 
                   IsMember=true,
                   MemberName= formatBuilder.FormatValue(value) 
                  }
                 }
                });
                return result;
            }
            else if (type.IsIn(typeof(DateTime))) 
            {
                Expression<Func<SingleColumnEntity<object>, object>> exp = it => Convert.ToDecimal(it.ColumnName);
                var result = queryBuilder.LambdaExpressions.DbMehtods.ToDate(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                  new MethodCallExpressionArgs(){
                   IsMember=true,
                   MemberName= formatBuilder.FormatValue(value)
                  }
                 }
                });
                return result;
            }
            else
            {
                return formatBuilder.FormatValue(value);
            }
        }

    }
}
