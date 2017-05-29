﻿using OrmTest.Models;
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
                #region StringIsNullOrEmpty
                StringIsNullOrEmpty();
                StringIsNullOrEmpty2();
                StringIsNullOrEmpty3();
                StringIsNullOrEmpty4();
                #endregion
                ToUpper();
                ToLower();
                Trim();
                Contains();
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
            }
            base.End("Method Test");
        }

        private void Length()
        {
            Expression<Func<Student, bool>> exp = it => NBORM.Length("aaaa") >1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(LEN(@MethodConst0) > @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),new SugarParameter("@Const1",1)
            }, "Length error");
        }

        private void Replace()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => NBORM.Replace("aaaa","a", "1") == "a";
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
            Expression<Func<Student, bool>> exp = it => NBORM.Substring("aaaa",0,2) == "a";
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToBool("true") == true;
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToDouble("2") == 2;
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToGuid("A94027A3-476E-478D-8228-F4054394B874") == x2;
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToDecimal("22") == 1;
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToString("2015-1-1") == "a";
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToDate("2015-1-1") == x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS DATETIME) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2015-1-1"),new SugarParameter("@Const1",x2)
            }, "ToDate error");
        }

        private void ToInt64()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.ToInt64("3") == 1;
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
            Expression<Func<Student, bool>> exp = it => NBORM.ToInt32("3")== 1;
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
            Expression<Func<Student, bool>> exp = it => NBORM.DateValue(x2,DateType.Year)==1;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ((@MethodConst1(@MethodConst0)) = @Const2 ) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",DateType.Year),new SugarParameter("@Const2",1)
            }, "DateValue error");
        }

        private void StartsWith()
        {
            Expression<Func<Student, bool>> exp = it => NBORM.StartsWith(it.Name, "a");
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
            Expression<Func<Student, bool>> exp = it => NBORM.EndsWith(it.Name, "a");
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
            Expression<Func<Student, bool>> exp = it => NBORM.Between(it.Name,1, 2);
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
            Expression<Func<Student, bool>> exp = it => NBORM.DateAdd(x2, 11, DateType.Millisecond)==x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(@MethodConst2,@MethodConst1,@MethodConst0)) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",11),new SugarParameter("@Const3",x2),
                new SugarParameter("@MethodConst2",DateType.Millisecond)
            }, "DateAddByType error");
        }
        private void DateAddDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateAdd(x2, 1)==x2;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATEADD(day,@MethodConst1,@MethodConst0)) = @Const2 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",1),new SugarParameter("@Const2",x2)
            }, "DateIsSameByType error");
        }

        private void DateIsSameByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateIsSame(x2,x2, DateType.Millisecond);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (DATEDIFF(@MethodConst2,@MethodConst0,@MethodConst1)=0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",x2),
                new SugarParameter("@MethodConst2",DateType.Millisecond)
            }, "DateIsSameByType error");
        }
        private void DateIsSameByDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateIsSame(x2,x2);
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
            Expression<Func<Student, bool>> exp = it => NBORM.Equals(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] = @MethodConst0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => NBORM.Equals("a",it.Name);
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
            Expression<Func<Student, bool>> exp = it => NBORM.Equals(it.Name, it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] = [Name]) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => NBORM.Equals("a", "a2");
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
            Expression<Func<Student, bool>> exp = it => NBORM.Contains(it.Name, "a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like '%'+@MethodConst0+'%') ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains error");
        }

        private void ContainsArray()
        {
            string[] array = new string[] { "1","2"};
            Expression<Func<Student, bool>> exp = it => NBORM.ContainsArray(array, it.Name);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, null, "  ([Name] IN ('1','2')) ",null, "Contains2 error");
        }

        private void Trim()
        {
            Expression<Func<Student, bool>> exp = it =>NBORM.Trim("  a")==it.Name;
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
            Expression<Func<Student, bool>> exp = it =>"a"== NBORM.ToUpper(it.Id) ;
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
            Expression<Func<Student, bool>> exp = it => "a" == NBORM.ToLower(it.Id);
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
            Expression<Func<Student, bool>> exp = it => it.Id > 2 || NBORM.IsNullOrEmpty(it.Id); ;
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
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(true); ;
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
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(a); ;
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
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(WhereConst.name); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = [Id] ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1","xx"),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty4 error");
        } 
        #endregion
    }
}

