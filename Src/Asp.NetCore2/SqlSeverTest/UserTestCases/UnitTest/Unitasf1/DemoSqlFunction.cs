using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SugarDbType = SqlSugar.DbType;

namespace SqlSugarCoreDemo.Demo
{
    public class DemoSqlFunction
    {
        public static bool MyContainsArrayUseSqlParameters<T>(List<T> thisValue, object InField)
        {
            //这里不能写任何实现代码，需要在上面的配置中实现
            throw new NotSupportedException("Can only be used in expressions");
        }

        private static List<SqlFuncExternal> _SqlFuncExternals;
        public static List<SqlFuncExternal> SqlFuncExternals()
        {
            if (_SqlFuncExternals != null)
            {
                return _SqlFuncExternals;
            }
            var expMethods = new List<SqlFuncExternal>
            {
                new SqlFuncExternal()
                {
                    UniqueMethodName = "MyContainsArrayUseSqlParameters",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == SugarDbType.SqlServer)
                        {
                            var inValueIEnumerable = (IEnumerable)expInfo.Args[0].MemberValue;
                            var inValueHashCode=inValueIEnumerable.GetHashCode();

                            var parp=expContext.Parameters.First(x=>x.Value.GetHashCode()==inValueHashCode);

                            var tempDt = new DataTable();
                            tempDt.Columns.Add("Value", typeof(long));
                            foreach (var item in inValueIEnumerable)
                            {
                                var r = tempDt.NewRow();
                                r["Value"] = item;
                                tempDt.Rows.Add(r);
                            }
                            //表结构变量
                            parp.TypeName="dbo.Long_Table";

                            parp.Value=tempDt;

                            return $"{expInfo.Args[1].MemberName} IN (SELECT Value FROM {parp.ParameterName})";
                        }
                        else
                            throw new Exception("未实现");
                    }
                }
            };

            return expMethods;
        }
    }
}
