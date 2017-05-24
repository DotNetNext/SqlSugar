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
    public class Where : UnitTestBase
    {
        private Where() { }
        public Where(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                whereSingle1();
                whereSingle2();
                whereSingle3();
                whereSingle4();
                whereSingle5();
                whereSingle6();
                whereSingle7(new Student() { Id = 1 });
                //whereSingle8(new Student() { Id = 1 });
                WhereMultiple1();
                WhereMultiple2();
          
            }
            base.End("Where Test");
        }
        private void WhereMultiple1()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [it].[Id] > @Id0 )", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, "WhereMultiple1");
        }
        private void WhereMultiple2()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name || it.Id == 1) || it.Name == WhereConst.name;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (((( [it].[Id] > @Id0 ) AND ( [it].[Name] <> @Name1 )) OR ( [it].[Id] = @Id2 )) OR ( [it].[Name] = @Name3 ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",1),
                new SugarParameter("@Name1","a"),
                new SugarParameter("@Id2",1),
                new SugarParameter("@Name3","a1")
            }, "WhereMultiple2");
        }
        private void whereSingle1()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [Id] > @Id0 )", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, "whereSingle1");
        }
        private void whereSingle2()
        {
            Expression<Func<Student, bool>> exp = it => 1 > it.Id;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Id0 > [Id] )", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, "whereSingle2");
        }
        private void whereSingle3()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1 || it.Name == "a";
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (( [Id] > @Id0 ) OR ( [Name] = @Name1 ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",1),
                new SugarParameter("@Name1","a")
            }, "whereSingle3");
        }
        private void whereSingle4()
        {
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != "a") || it.Name == "a1";
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ((( [Id] > @Id0 ) AND ( [Name] <> @Name1 )) OR ( [Name] = @Name2 ))  ", new List<SugarParameter>() {
                new SugarParameter("@Id0",1),
                new SugarParameter("@Name1","a"),
                new SugarParameter("@Name2","a1")
            }, "whereSingle4");
        }
        private void whereSingle5()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name) || it.Name == WhereConst.name;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ((( [Id] > @Id0 ) AND ( [Name] <> @Name1 )) OR ( [Name] = @Name2 ))  ", new List<SugarParameter>() {
                new SugarParameter("@Id0",1),
                new SugarParameter("@Name1","a"),
                new SugarParameter("@Name2","a1")
            }, "whereSingle5");
        }
        private void whereSingle6()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name||it.Id==1) || it.Name == WhereConst.name;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (((( [Id] > @Id0 ) AND ( [Name] <> @Name1 )) OR ( [Id] = @Id2 )) OR ( [Name] = @Name3 ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",1),
                new SugarParameter("@Name1","a"),
                new SugarParameter("@Id2",1),
                new SugarParameter("@Name3","a1")
            }, "whereSingle6");
        }
        private void whereSingle7(Student st)
        {
            Expression<Func<Student, bool>> exp = it => it.Id > st.Id;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [Id] > @Id0 )", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, "whereSingle7");
        }

        private void whereSingle8(Student st)
        {
            Expression<Func<Student, bool>> exp = it => it.Name == null;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( [Name] is @Id0 )", new List<SugarParameter>() {
                new SugarParameter("@Id0",1)
            }, "whereSingle8");
        }
    }

    public class WhereConst
    {
        public static string name { get; set; }
    }
}
