using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Method : UnitTestBase
    {
        private Method() { }
        public Method(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {

                //Native methods
                ExtendContainsArray();
                ConvetToString();
                ExtendToString();
                ExtendSubstring();
                ExtendDate();

                //SqlFun methods
                MappingColumn();
                IIF();
                IIF2();
                IIF3();
                IIF4();
                IIF5();
                #region StringIsNullOrEmpty
                HasValue();
                HasNumber();
                StringIsNullOrEmpty();
                StringIsNullOrEmpty2();
                StringIsNullOrEmpty3();
                StringIsNullOrEmpty4();
                StringIsNullOrEmpty5();
                #endregion
                ToUpper();
                ToLower();
                Trim();
                Contains();
                Contains2();
                Contains3();
                ContainsArray();
                StartsWith();
                EndsWith();
                Between();
                Equals();
                Equals_2();
                DateIsSameByDay();
                DateIsSameByType();
                DateAddDay();
                DateAddByType();
                DateValue();
                ToInt32();
                ToInt64();
                ToDate();
                Tostring();
                ToDecimal();
                ToGuid();
                ToDouble();
                ToBool();
                Substring();
                Replace();
                Length();
                Time();

                Test1();
            }
            base.End("Method Test");
        }

        private void Test1()
        {
            var ids = new int[] { 1, 2, 3 };
            Expression<Func<Student, bool>> exp = it => ids.Contains(it.Id)&&!SqlFunc.IsNullOrEmpty(it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(([Id] IN ('1','2','3')) AND NOT( [Name]='' OR [Name] IS NULL ))", new List<SugarParameter>() {
           
            }, "Test1 error");
        }

        private void ExtendToString()
        {
            Expression<Func<Student, bool>> exp = it => it.Id.ToString() == "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST([Id] AS NVARCHAR(MAX)) = @Const0 )", new List<SugarParameter>() {
                 new SugarParameter("@Const0","a")
            }, "ExtendToString error");
        }

        private void ConvetToString()
        {
            Expression<Func<Student, bool>> exp = it => Convert.ToString(it.Id) == "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST([Id] AS NVARCHAR(MAX)) = @Const0 )", new List<SugarParameter>() {
                 new SugarParameter("@Const0","a")
            }, "ConvetToString error");
        }


        private void Length()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Length("aaaa") > 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(LEN(@MethodConst0) > @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),new SugarParameter("@Const1",1)
            }, "Length error");

            Length2();
            Length3();
        }

        private void Length2()
        {
            Expression<Func<Student, bool>> exp = it => it.Name.Length > 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(LEN([Name])> @Length1 )", new List<SugarParameter>() {
                new SugarParameter("@Length1",1) 
            }, "Length2 error");
        }

        private void Length3()
        {
            Expression<Func<Student, bool>> exp = it => it.Name.Length > "a".Length;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(LEN([Name])> @Length1 )", new List<SugarParameter>() {
                new SugarParameter("@Length1",1)
            }, "Length3 error");
        }

        private void Time()
        {
            TimeSpan s = TimeSpan.Parse("11:22:22");
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToTime("11:12:59")==s;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS TIME) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","11:12:59"),new SugarParameter("@Const1",s)
            }, "Time error");
        }


        private void Replace()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.Replace("aaaa", "a", "1") == "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(REPLACE(@MethodConst0,@MethodConst1,@MethodConst2) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),   new SugarParameter("@MethodConst1","a") ,   new SugarParameter("@MethodConst2","1"),new SugarParameter("@Const3","a")
            }, "Replace error");
        }

        private void Substring()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.Substring("aaaa", 0, 2) == "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(SUBSTRING(@MethodConst0,1 + @MethodConst1,@MethodConst2) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),   new SugarParameter("@MethodConst1",0) ,   new SugarParameter("@MethodConst2",2),new SugarParameter("@Const3","a")
            }, "Substring error");
        }
        private void ExtendSubstring()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it =>"aaaa".Substring(0, 2)== "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(SUBSTRING(@MethodConst0,1 + @MethodConst1,@MethodConst2) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),   new SugarParameter("@MethodConst1",0) ,   new SugarParameter("@MethodConst2",2),new SugarParameter("@Const3","a")
            }, "Substring error");
        }
        

        private void ToBool()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToBool("true") == true;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS BIT) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","true"),new SugarParameter("@Const1",(bool)true)
            }, "ToBool error");
        }

        private void ToDouble()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDouble("2") == 2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS FLOAT) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2"),new SugarParameter("@Const1",(Double)2)
            }, "ToDouble error");
        }

        private void ToGuid()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToGuid("A94027A3-476E-478D-8228-F4054394B874") == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS UNIQUEIDENTIFIER) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","A94027A3-476E-478D-8228-F4054394B874"),new SugarParameter("@Const1",x2)
            }, "ToGuid error");
        }

        private void ToDecimal()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDecimal("22") == 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS MONEY) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","22"),new SugarParameter("@Const1",(decimal)1)
            }, "ToDecimal error");
        }

        private void Tostring()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToString("2015-1-1") == "a";
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS NVARCHAR(MAX)) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2015-1-1"),new SugarParameter("@Const1","a")
            }, "Tostring error");
        }

        private void ToDate()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDate("2015-1-1") == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS DATETIME) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2015-1-1"),new SugarParameter("@Const1",x2)
            }, "ToDate error");
        }
        private void ExtendDate()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => Convert.ToDateTime("2015-1-1") == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS DATETIME) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2015-1-1"),new SugarParameter("@Const1",x2)
            }, "ExtendDate error");
        }
        
        private void ToInt64()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToInt64("3") == 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS BIGINT) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","3"),new SugarParameter("@Const1",(Int64)1)
            }, "ToInt64 error");
        }

        private void ToInt32()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToInt32("3") == 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS INT) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","3"),new SugarParameter("@Const1",1)
            }, "ToInt32 error");
        }

        private void DateValue()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateValue(x2, DateType.Year) == 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (DateName(Year,@MethodConst0) = @Const2 ) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@Const2",1)
            }, "DateValue error");
        }

        private void StartsWith()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.StartsWith(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like @MethodConst0+'%') ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "StartsWith error");
        }
        private void EndsWith()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.EndsWith(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like '%'+@MethodConst0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "EndsWith");
        }
        private void Between()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Between(it.Name, 1, 2);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] BETWEEN @MethodConst0 AND @MethodConst1) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",1),new SugarParameter("@MethodConst1",2),
            }, "Between error");
        }

        private void DateAddByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateAdd(x2, 11, DateType.Millisecond) == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(Millisecond,@MethodConst1,@MethodConst0)) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",11),new SugarParameter("@Const3",x2)
         
            }, "DateAddByType error");
        }
        private void DateAddDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateAdd(x2, 1) == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(day,@MethodConst1,@MethodConst0)) = @Const2 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",1),new SugarParameter("@Const2",x2)
            }, "DateAddDay error");

            DateAddDay2();
            DateAddDay3();
        }

        private void DateAddDay2()
        {
            var x2 = DateTime.Now;
            Expression<Func<DataTestInfo, bool>> exp = it =>it.Datetime2.Value.AddHours(10) == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(Hour,@MethodConst1,[Datetime2])) = @Const2 )", new List<SugarParameter>() {
                 new SugarParameter("@MethodConst1",10) 
                ,new SugarParameter("@Const2",x2)
            }, "DateAddDay2 error");
        }

        private void DateAddDay3()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => x2.AddHours(1) == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(Hour,@MethodConst2,@MethodConst1)) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst2",1),
                new SugarParameter("@MethodConst1",x2)
                ,new SugarParameter("@Const3",x2)
            }, "DateAddDay3 error");
        }

        private void DateIsSameByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateIsSame(x2, x2, DateType.Millisecond);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (DATEDIFF(Millisecond,@MethodConst0,@MethodConst1)=0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",x2)
            }, "DateIsSameByType error");
        }
        private void DateIsSameByDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateIsSame(x2, x2);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(DATEDIFF(day,@MethodConst0,@MethodConst1)=0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",x2)
            }, "DateIsSameDay error");
        }

        private void Equals()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Equals(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] = @MethodConst0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => SqlFunc.Equals("a", it.Name);
            SqlServerExpressionContext expContext2 = new SqlServerExpressionContext();
            expContext2.Resolve(exp2, ResolveExpressType.WhereSingle);
            var value2 = expContext2.Result.GetString();
            var pars2 = expContext2.Parameters;
            base.Check(value2, pars2, " (@MethodConst0 = [Name]) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals2 error");
        }
        private void Equals_2()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Equals(it.Name, it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] = [Name]) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => SqlFunc.Equals("a", "a2");
            SqlServerExpressionContext expContext2 = new SqlServerExpressionContext();
            expContext2.Resolve(exp2, ResolveExpressType.WhereSingle);
            var value2 = expContext2.Result.GetString();
            var pars2 = expContext2.Parameters;
            base.Check(value2, pars2, " (@MethodConst0 = @MethodConst1) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a"),new SugarParameter("@MethodConst1","a2")
            }, "Equals2 error");
        }

        private void Contains()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Contains(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like '%'+@MethodConst0+'%') ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains error");
        }
        private void Contains2(string name="a")
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Contains(it.Name, name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like '%'+@MethodConst0+'%') ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains2 error");
        }
        private void Contains3(string name = "a")
        {
            Expression<Func<Student, bool>> exp = it => !SqlFunc.Contains(it.Name, name)&&it.Id==1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(NOT ([Name] like '%'+@MethodConst0+'%')  AND( [Id] = @Id1 ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a"),
                 new SugarParameter("@Id1",1)
            }, "Contains3 error");
        }

        private void ExtendContainsArray() {
            var array = new string[] { "1", "2" }.ToList();
            Expression<Func<Student, bool>> exp = it => array.Contains(it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, null, "  ([Name] IN ('1','2')) ", null, "Contains2 error");
        }

        private void ContainsArray()
        {
            string[] array = new string[] { "1", "2" };
            Expression<Func<Student, bool>> exp = it => SqlFunc.ContainsArray(array, it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, null, "  ([Name] IN ('1','2')) ", null, "Contains2 error");
        }

        private void Trim()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Trim("  a") == it.Name;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((rtrim(ltrim(@MethodConst0))) = [Name] )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","  a")
            }, "Trim error");
        }

        private void ToUpper()
        {
            Expression<Func<Student, bool>> exp = it => "a" == SqlFunc.ToUpper(it.Id);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0 = (UPPER([Id])) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToUpper error");
        }
        private void ToLower()
        {
            Expression<Func<Student, bool>> exp = it => "a" == SqlFunc.ToLower(it.Id);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0 = (LOWER([Id])) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToLower error");
        }

        #region StringIsNullOrEmpty
        private void StringIsNullOrEmpty()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 2 || SqlFunc.IsNullOrEmpty(it.Id); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( [Id] > @Id0 ) OR ( [Id]='' OR [Id] IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty error");
        }
        private void StringIsNullOrEmpty2()
        {
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(true); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = [Id] ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",true),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty2 error");
        }
        private void StringIsNullOrEmpty3()
        {
            int a = 1;
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(a); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = [Id] ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",1),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty3 error");
        }
        private void StringIsNullOrEmpty4()
        {
            WhereConst.name = "xx";
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(WhereConst.name); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = [Id] ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1","xx"),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty4 error");
        }
        private void StringIsNullOrEmpty5()
        {
            WhereConst.name = "xx";
            Expression<Func<Student, bool>> exp = it => !SqlFunc.IsNullOrEmpty(WhereConst.name); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "NOT( @MethodConst0='' OR @MethodConst0 IS NULL )", new List<SugarParameter>() {
               new SugarParameter("@MethodConst0","xx")
            }, "StringIsNullOrEmpty5 error");
        }
        #endregion

        private void HasValue()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.HasValue(it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [Name]<>'' AND [Name] IS NOT NULL )", new List<SugarParameter>()
            {

            }, "HasValue error");
            HasValue2(1);
        }
        private void HasValue2(int p = 1)
        {
            Expression<Func<Student, bool>> exp = it => it.CreateTime.HasValue;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"( [it].[CreateTime]<>'' AND [it].[CreateTime] IS NOT NULL )", null, selectorValue, null,
                "Select.HasValue2 Error");
        }
        private void HasNumber()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.HasNumber(it.Id);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [Id]>0 AND [Id] IS NOT NULL )", new List<SugarParameter>()
            {


            }, "HasNumber error");
        }
        private void MappingColumn() {
            Expression<Func<Student, bool>> exp = it => SqlFunc.MappingColumn(it.Id,"Name") == 1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(Name = @Const1 )", new List<SugarParameter>()
            {
                     new SugarParameter("@Const1",1)
            }, "MappingColumn error");
        }
        private void IIF()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.IIF(it.Id == 1, 1, 2)==1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN ( [Id] = @Id0 ) THEN @MethodConst1  ELSE @MethodConst2 END ) = @Const3 )", new List<SugarParameter>()
            {
                     new SugarParameter("@Id0",1),
                     new SugarParameter("@MethodConst1",1),
                     new SugarParameter("@MethodConst2",2),
                     new SugarParameter("@Const3",1)
            }, "IIF error");
        }

        private void IIF2()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.IIF(SqlFunc.Contains(it.Name,"a"), 1, 2)==1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN  ([Name] like '%'+@MethodConst0+'%')  THEN @MethodConst1  ELSE @MethodConst2 END ) = @Const3 )", new List<SugarParameter>()
            {
                     new SugarParameter("@MethodConst0","a"),
                     new SugarParameter("@MethodConst1",1),
                     new SugarParameter("@MethodConst2",2),
                                     new SugarParameter("@Const3",1)
            }, "IIF2 error");
        }

        private void IIF3()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.IIF(SqlFunc.Contains(it.Name, "a"), true, false) == true;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN  ([Name] like '%'+@MethodConst0+'%')  THEN @MethodConst1  ELSE @MethodConst2 END ) = @Const3 )", new List<SugarParameter>()
            {
                     new SugarParameter("@MethodConst0","a"),
                     new SugarParameter("@MethodConst1",true),
                     new SugarParameter("@MethodConst2",false),
                                     new SugarParameter("@Const3",true)
            }, "IIF3 error");
        }

        private void IIF4()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => SqlFunc.IIF(true, it.Bool1, it.Bool2) == true;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN ( 1 = 1 )  THEN [Bool1]  ELSE [Bool2] END ) = @Const0 )", new List<SugarParameter>()
            {
                     new SugarParameter("@Const0",true)
            }, "IIF4 error");
        }

        private void IIF5()
        {
            Expression<Func<DataTestInfo, bool>> exp = it => SqlFunc.IIF(true,Convert.ToBoolean(it.Datetime1), SqlFunc.ToBool(it.Datetime1)) == false;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN ( 1 = 1 )  THEN  CAST([Datetime1] AS BIT)  ELSE  CAST([Datetime1] AS BIT) END ) = @Const0 )", new List<SugarParameter>()
            {
                     new SugarParameter("@Const0",false)
            }, "IIF5 error");
            IIF6();
        }

        private void IIF6()
        {
            var dt = DateTime.Now.Date;
            Expression <Func<DataTestInfo, bool>> exp = it => SqlFunc.IIF(dt == it.Datetime1,it.Datetime1, it.Datetime1) == dt;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.MappingColumns = new MappingColumnList();
            expContext.MappingColumns.Add("Datetime1", "Datetime2", "DataTestInfo");
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN ( @Datetime10 = [Datetime2] ) THEN [Datetime2]  ELSE [Datetime2] END ) = @Const1 )", new List<SugarParameter>()
            {
                     new SugarParameter("@Datetime10",dt),
                     new SugarParameter("@Const1",dt)
            }, "IIF6 error");
        }
    }
}

