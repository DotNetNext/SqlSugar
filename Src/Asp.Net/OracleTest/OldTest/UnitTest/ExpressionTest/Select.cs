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
                single2();
                single3();
                single4();
                single5();
                Multiple();
                Multiple2();
                singleDynamic();
                MultipleDynamic();
            }
            base.End("Select Test");
        }

        private void Multiple()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new Student() { Name = "a", Id = it.Id, SchoolId = school.Id, TestId = it.Id + 1 };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"  :CONSTANT0 AS ""NAME"" , ""IT"".""ID"" AS ""ID"" , ""SCHOOL"".""ID"" AS ""SCHOOLID"" , ( ""IT"".""ID"" + :ID1 ) AS ""TESTID"" ",
                new List<SugarParameter>(){
                 new SugarParameter(":constant0","a"),
                 new SugarParameter(":Id1",1)
                },
                "Select.Multiple Error");
        }
        private void Multiple2()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new ViewModelStudent3() { SchoolName = school.Name, Id = SqlFunc.GetSelfAndAutoFill(it.Id) };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" ""SCHOOL"".""NAME"" AS ""SCHOOLNAME"" ,it.*",
                new List<SugarParameter>()
                {

                },
                "Select.Multiple Error");
        }


        private void MultipleDynamic()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new { Name = "a", Id = it.Id / 2, SchoolId = school.Id };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
              selectorValue,
              pars,
              @" :CONSTANT0 AS ""NAME"" , ( ""IT"".""ID"" / :ID1 ) AS ""ID"" , ""SCHOOL"".""ID"" AS ""SCHOOLID""  ",
              new List<SugarParameter>(){
                new SugarParameter(":constant0","a"),
                new SugarParameter(":Id1", 2)},
              "Select.MultipleDynamic Error");
        }
        private void single()
        {
            int p = 1;
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p, TestId = it.Id + 11 };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" :CONSTANT0 AS ""NAME"" , ""ID"" AS ""ID"" , :CONSTANT1 AS ""SCHOOLID"" , ( ""ID"" + :ID2 ) AS ""TESTID""  ",
                new List<SugarParameter>(){
                            new SugarParameter(":constant0","a"),
                            new SugarParameter(":constant1",1),
                            new SugarParameter(":Id2",11 ) },
                "Select.single Error");
        }
        private void single2(int p = 1)
        {
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p, TestId = it.Id + 11 };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" :CONSTANT0 AS ""NAME"" , ""ID"" AS ""ID"" , :CONSTANT1 AS ""SCHOOLID"" , ( ""ID"" + :ID2 ) AS ""TESTID""  ",
                new List<SugarParameter>(){
                            new SugarParameter(":constant0","a"),
                            new SugarParameter(":constant1",1),
                            new SugarParameter(":Id2",11 ) },
                "Select.single Error");
        }
        private void single3(int p = 1)
        {
            Expression<Func<Student, object>> exp = it => new DataTestInfo() { Datetime1 = DateTime.Now, String = it.Name };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"  :CONSTANT0 AS ""DATETIME1"" , ""NAME"" AS ""STRING"" ", null, selectorValue, null,
                "Select.single3 Error");
        }

        private void single4(int p = 1)
        {
            Expression<Func<Student, object>> exp = it => it.CreateTime.HasValue;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"( ""IT"".""CREATETIME""<>'' AND ""IT"".""CREATETIME"" IS NOT NULL )", null, selectorValue, null,
                "Select.single4 Error");
        }

        private void single5()
        {
            var p = (DateTime?)DateTime.Now;
            Expression<Func<Student, object>> exp = it => p.HasValue;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"( :constant0<>'' AND :constant0 IS NOT NULL )", new List<SugarParameter>() {
                    new SugarParameter(":constant0",p)
                }, selectorValue, pars,
                "Select.single4 Error");
        }

        private void singleDynamic()
        {
            string a = "a";
            Expression<Func<Student, object>> exp = it => new { x = it.Id, shoolid = 1, name = a, p = it.Id * 2 };
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
            selectorValue,
            pars,
            @" ""ID"" AS ""X"" , :CONSTANT0 AS ""SHOOLID"" , :CONSTANT1 AS ""NAME"" , ( ""ID"" * :ID2 ) AS ""P"" ",
            new List<SugarParameter>(){
                                    new SugarParameter(":constant0",1),
                                    new SugarParameter(":constant1","a"),
                                    new SugarParameter(":Id2",2)},
            "Select.singleDynamic Error");
        }
    }
}
