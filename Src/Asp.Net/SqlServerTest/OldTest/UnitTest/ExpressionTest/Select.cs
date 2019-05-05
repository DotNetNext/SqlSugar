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
                single6();
                single7();
                single8();
                Multiple();
                Multiple2();
                singleDynamic();
                MultipleDynamic();
            }
            base.End("Select Test");
        }

        private void single7()
        {
            Expression<Func<DataTestInfo2, DataTestInfo2>> exp =it => new DataTestInfo2() {  Bool1=it.Bool1 , Bool2=it.Bool2 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"[Bool1] AS [Bool1] , [Bool2] AS [Bool2] ",
                new List<SugarParameter>()
                {

                },
                "Select.single7 Error");
        }

        private void single8()
        {
            Expression<Func<DataTestInfo2, object>> exp = it => new  { Bool1 = it.Bool1, Bool2 = it.Bool2 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"[Bool1] AS [Bool1] , [Bool2] AS [Bool2] ",
                new List<SugarParameter>()
                {

                },
                "Select.single8 Error");
        }

        private void Multiple()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new Student() { Name = "a", Id = it.Id, SchoolId = school.Id, TestId = it.Id + 1 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"  @constant0 AS [Name] , [it].[Id] AS [Id] , [school].[Id] AS [SchoolId] , ( [it].[Id] + @Id1 ) AS [TestId] ",
                new List<SugarParameter>(){
                 new SugarParameter("@constant0","a"),
                 new SugarParameter("@Id1",1)
                },
                "Select.Multiple Error");
        }
        private void Multiple2()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new ViewModelStudent3() {  SchoolName=school.Name,Id=SqlFunc.GetSelfAndAutoFill(it.Id) };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" [school].[Name] AS [SchoolName] ,it.*",
                new List<SugarParameter>(){
                
                },
                "Select.Multiple Error");
        }


        private void MultipleDynamic()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new { Name = "a", Id = it.Id / 2, SchoolId = school.Id };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
              selectorValue,
              pars,
              @" @constant0 AS [Name] , ( [it].[Id] / @Id1 ) AS [Id] , [school].[Id] AS [SchoolId]  ",
              new List<SugarParameter>(){
                new SugarParameter("@constant0","a"),
                new SugarParameter("@Id1", 2)},
              "Select.MultipleDynamic Error");
        }
        private  void single()
        {
            int p = 1;
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p,TestId=it.Id+11 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" @constant0 AS [Name] , [Id] AS [Id] , @constant1 AS [SchoolId] , ( [Id] + @Id2 ) AS [TestId]  ",
                new List<SugarParameter>(){
                            new SugarParameter("@constant0","a"),
                            new SugarParameter("@constant1",1),
                            new SugarParameter("@Id2",11 ) },
                "Select.single Error");
        }
        private void single2(int p=1)
        {
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p, TestId = it.Id + 11 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" @constant0 AS [Name] , [Id] AS [Id] , @constant1 AS [SchoolId] , ( [Id] + @Id2 ) AS [TestId]  ",
                new List<SugarParameter>(){
                            new SugarParameter("@constant0","a"),
                            new SugarParameter("@constant1",1),
                            new SugarParameter("@Id2",11 ) },
                "Select.single Error");
        }
        private void single3(int p = 1)
        {
            Expression<Func<Student, object>> exp = it => new DataTestInfo() { Datetime1=DateTime.Now,  String=it.Name};
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"  @constant0 AS [Datetime1] , [Name] AS [String] ", null,selectorValue,null,
                "Select.single3 Error");
        }

        private void single4(int p = 1)
        {
            Expression<Func<Student, object>> exp = it => it.CreateTime.HasValue;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"( [it].[CreateTime]<>'' AND [it].[CreateTime] IS NOT NULL )", null, selectorValue, null,
                "Select.single4 Error");
        }

        private void single5()
        {
            var p =(DateTime?) DateTime.Now;
            Expression<Func<Student, object>> exp = it => p.HasValue;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @"( @constant0<>'' AND @constant0 IS NOT NULL )", new List<SugarParameter>() {
                    new SugarParameter("@constant0",p)
                }, selectorValue, pars,
                "Select.single5 Error");
        }
        private void single6()
        {
            var p = (DateTime?)DateTime.Now;
            Expression<Func<Student, object>> exp = it => p.Value;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.FieldSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                @" @Const0 ", new List<SugarParameter>() {
                    new SugarParameter("@Const0",p)
                }, selectorValue, pars,
                "Select.single6 Error");
        }

        private  void singleDynamic()
        {
            string a = "a";
            Expression<Func<Student, object>> exp = it => new { x = it.Id, shoolid = 1, name = a,p=it.Id*2 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
            selectorValue,
            pars,
            @" [Id] AS [x] , @constant0 AS [shoolid] , @constant1 AS [name] , ( [Id] * @Id2 ) AS [p] ",
            new List<SugarParameter>(){
                                    new SugarParameter("@constant0",1),
                                    new SugarParameter("@constant1","a"),
                                    new SugarParameter("@Id2",2)},
            "Select.singleDynamic Error");
        }
    }
}
