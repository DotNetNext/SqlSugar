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
    public class Select : UnitTestBase
    {
        private Select() { }
        public Select(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                single();
                Multiple();
                singleDynamic();
                MultipleDynamic();
            }
            base.End("Select Test");
        }

        private void Multiple()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new Student() { Name = "a", Id = it.Id, SchoolId = school.Id, TestId = it.Id + 1 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" @constant1 AS Name , it.Id AS Id , school.Id AS SchoolId , ( it.Id + 1 )   AS TestId ",
                new List<SugarParameter>(){
                new SugarParameter("@constant1","a")},
                "Select.Multiple Error");
        }

        private  void MultipleDynamic()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new { Name = "a", Id = it.Id / 2, SchoolId = school.Id };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
              selectorValue,
              pars,
              @"  @constant1 AS Name , ( it.Id / 2 )   AS Id , school.Id AS SchoolId  ",
              new List<SugarParameter>(){
                new SugarParameter("@constant1","a")},
              "Select.MultipleDynamic Error");
        }
        private  void single()
        {
            int p = 1;
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p,TestId=it.Id+1 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"  @constant1 AS Name , Id AS Id , @constant3 AS SchoolId , ( Id + 1 )   AS TestId  ",
                new List<SugarParameter>(){
                            new SugarParameter("@constant1","a"),
                            new SugarParameter("@constant3",1)},
                "Select.single Error");
        }

        private  void singleDynamic()
        {
            string a = "a";
            Expression<Func<Student, object>> exp = it => new { x = it.Id, shoolid = 1, name = a,p=it.Id*1 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
            selectorValue,
            pars,
            @" Id AS x , @constant2 AS shoolid , @constant3 AS name , ( Id * 1 )   AS p  ",
            new List<SugarParameter>(){
                                    new SugarParameter("@constant2",1),
                                    new SugarParameter("@constant3","a")},
            "Select.single Error");
        }
    }
}
