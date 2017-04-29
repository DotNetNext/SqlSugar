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
    public class Method : ExpTestBase
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
                ToUpper();
                ToLower();
                Trim();
                Contains();
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
                #endregion
            }
            base.End("Method Test");
        }

        private void DateValue()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateValue(x2,DateType.Year);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "  (@MethodConst1(@MethodConst0)) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",DateType.Year)
            }, "StartsWith");
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
            }, "StartsWith");
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
            }, "Between");
        }

        private void DateAddByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateAdd(x2, 11, DateType.Millisecond);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (DATEADD(@MethodConst2,@MethodConst1,@MethodConst0)) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",11),
                new SugarParameter("@MethodConst2",DateType.Millisecond)
            }, "DateAddByType");
        }
        private void DateAddDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => NBORM.DateAdd(x2, 1);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (DATEADD(day,@MethodConst1,@MethodConst0)) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",1)
            }, "DateIsSameByType");
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
            }, "DateIsSameByType");
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
            }, "DateIsSameDay");
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
            }, "Equals1");


            Expression<Func<Student, bool>> exp2 = it => NBORM.Equals("a",it.Name);
            SqlServerExpressionContext expContext2 = new SqlServerExpressionContext();
            expContext2.Resolve(exp2, ResolveExpressType.WhereSingle);
            var value2 = expContext2.Result.GetString();
            var pars2 = expContext2.Parameters;
            base.Check(value2, pars2, " (@MethodConst0 = [Name]) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals2");
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
            }, "Equals1");


            Expression<Func<Student, bool>> exp2 = it => NBORM.Equals("a", "a2");
            SqlServerExpressionContext expContext2 = new SqlServerExpressionContext();
            expContext2.Resolve(exp2, ResolveExpressType.WhereSingle);
            var value2 = expContext2.Result.GetString();
            var pars2 = expContext2.Parameters;
            base.Check(value2, pars2, " (@MethodConst0 = @MethodConst1) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a"),new SugarParameter("@MethodConst1","a2")
            }, "Equals2");
        }

        private void Contains()
        {
            Expression<Func<Student, bool>> exp = it => NBORM.Contains(it.Name,"a");
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ([Name] like '%'+@MethodConst0+'%') ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains");
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
            }, "Trim");
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
            }, "ToUpper");
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
            }, "ToLower");
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
            }, "StringIsNullOrEmpty");
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
            }, "StringIsNullOrEmpty2");
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
            }, "StringIsNullOrEmpty3");
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
            }, "StringIsNullOrEmpty4");
        } 
        #endregion
    }
}

