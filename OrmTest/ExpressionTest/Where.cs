using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.ExpressionTest
{
    public class Where : ExpTestBase
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
            }
            base.End("Where Test");
        }

        private void whereSingle1()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1;
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ( Id  > @Id1 ) ", new List<SugarParameter>() {
                new SugarParameter("@Id1",1)
            }, "whereSingle1");
        }
        private void whereSingle2()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1 || it.Name == "a";
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "  ( Id  > @Id1 )  OR  ( Name  = @Name2 )  ", new List<SugarParameter>() {
                new SugarParameter("@Id1",1),
                new SugarParameter("@Name2","a")
            }, "whereSingle2");
        }
        private void whereSingle3()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1 || it.Name == "a";
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "  ( Id  > @Id1 )  OR  ( Name  = @Name2 )  ", new List<SugarParameter>() {
                new SugarParameter("@Id1",1),
                new SugarParameter("@Name2","a")
            }, "whereSingle2");
        }
        private void whereSingle4()
        {
            Expression<Func<Student, bool>> exp = it =>( it.Id > 1 &&it.Name!="a")|| it.Name == "a";
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "  ( Id  > @Id1 )  OR  ( Name  = @Name2 )  ", new List<SugarParameter>() {
                new SugarParameter("@Id1",1),
                new SugarParameter("@Name2","a")
            }, "whereSingle4");
        }
    }
}
