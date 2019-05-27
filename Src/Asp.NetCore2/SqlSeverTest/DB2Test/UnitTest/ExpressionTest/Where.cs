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

                whereSingle23();
                whereSingle22();
                whereSingle21();
                whereSingle20();
                whereSingle19();
                whereSingle18();
                whereSingle17();
                whereSingle16();
                whereSingle15();
                whereSingle1();
                whereSingle2();
                whereSingle3();
                whereSingle4();
                whereSingle5();
                whereSingle6();
                whereSingle7(new Student() { Id = 1 });
                whereSingle8(new Student() { Id = 1 });
                whereSingle9(new Student() { Id = 1 });
                whereSingle10();
                whereSingle11();
                whereSingle12();
                whereSingle13();
                whereSingle14();
                whereSingle15();
                WhereMultiple1();
                WhereMultiple2();
          
            }
            base.End("Where Test");
        }
        private void WhereMultiple1()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1;
            OracleExpressionContext expContext = new  OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"it\".\"Id\" > :Id0 )", new List<SugarParameter>() {
                new SugarParameter(":Id0",1)
            }, "WhereMultiple1");
        }
        private void WhereMultiple2()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name || it.Id == 1) || it.Name == WhereConst.name;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereMultiple);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (((( \"it\".\"Id\" > :Id0 ) AND ( \"it\".\"Name\" <> :Name1 )) OR ( \"it\".\"Id\" = :Id2 )) OR ( \"it\".\"Name\" = :Name3 ))", new List<SugarParameter>() {
                new SugarParameter(":Id0",1),
                new SugarParameter(":Name1","a"),
                new SugarParameter(":Id2",1),
                new SugarParameter(":Name3","a1")
            }, "WhereMultiple2");
        }
        private void whereSingle1()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"Id\" > :Id0 )", new List<SugarParameter>() {
                new SugarParameter(":Id0",1)
            }, "whereSingle1");
        }
        private void whereSingle2()
        {
            Expression<Func<Student, bool>> exp = it => 1 > it.Id;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( :Id0 > \"Id\" )", new List<SugarParameter>() {
                new SugarParameter(":Id0",1)
            }, "whereSingle2");
        }
        private void whereSingle3()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 1 || it.Name == "a";
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (( \"Id\" > :Id0 ) OR ( \"Name\" = :Name1 ))", new List<SugarParameter>() {
                new SugarParameter(":Id0",1),
                new SugarParameter(":Name1","a")
            }, "whereSingle3");
        }
        private void whereSingle4()
        {
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != "a") || it.Name == "a1";
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ((( \"Id\" > :Id0 ) AND ( \"Name\" <> :Name1 )) OR ( \"Name\" = :Name2 ))  ", new List<SugarParameter>() {
                new SugarParameter(":Id0",1),
                new SugarParameter(":Name1","a"),
                new SugarParameter(":Name2","a1")
            }, "whereSingle4");
        }
        private void whereSingle5()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name) || it.Name == WhereConst.name;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " ((( \"Id\" > :Id0 ) AND ( \"Name\" <> :Name1 )) OR ( \"Name\" = :Name2 ))  ", new List<SugarParameter>() {
                new SugarParameter(":Id0",1),
                new SugarParameter(":Name1","a"),
                new SugarParameter(":Name2","a1")
            }, "whereSingle5");
        }
        private void whereSingle6()
        {
            string name = "a";
            WhereConst.name = "a1";
            Expression<Func<Student, bool>> exp = it => (it.Id > 1 && it.Name != name||it.Id==1) || it.Name == WhereConst.name;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (((( \"Id\" > :Id0 ) AND ( \"Name\" <> :Name1 )) OR ( \"Id\" = :Id2 )) OR ( \"Name\" = :Name3 ))", new List<SugarParameter>() {
                new SugarParameter(":Id0",1),
                new SugarParameter(":Name1","a"),
                new SugarParameter(":Id2",1),
                new SugarParameter(":Name3","a1")
            }, "whereSingle6");
        }
        private void whereSingle7(Student st)
        {
            Expression<Func<Student, bool>> exp = it => it.Id > st.Id;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"ID\" > :Id0 )", new List<SugarParameter>() {
                new SugarParameter(":Id0",1)
            }, "whereSingle7");
        }

        private void whereSingle8(Student st)
        {
            Expression<Func<Student, bool>> exp = it => it.Name == null;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"NAME\" IS NULL )", new List<SugarParameter>() {

            }, "whereSingle8");
        }

        private void whereSingle9(Student st)
        {
            Expression<Func<Student, bool>> exp = it => it.Name == st.Name;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"NAME\" = :Name0 )", new List<SugarParameter>()
            {
                new SugarParameter(":Name0",null)
            }, "whereSingle9");
        }


        private void whereSingle10()
        {
            Expression<Func<Student, bool>> exp = it => true;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( 1 = 1 )", new List<SugarParameter>()
            {
              
            }, "whereSingle10");
        }


        private void whereSingle11()
        {
            Expression<Func<Student, bool>> exp = it => !true;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( 1 = 2 )", new List<SugarParameter>()
            {

            }, "whereSingle11");
        }

        private void whereSingle12()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => it.Bool1==true;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"BOOL1\" = :Bool10 )", new List<SugarParameter>()
            {
                new SugarParameter(":Bool10",true)
            }, "whereSingle12");
        }

        private void whereSingle13()
        {
            Expression<Func<Student, bool>> exp = it => it.Name!=null;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"NAME\" IS NOT NULL )", new List<SugarParameter>()
            {
 
            }, "whereSingle13");
        }

        private void whereSingle14()
        {
            Expression<Func<Student, bool>> exp = it =>true&& it.Name != null;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( 1 = 1 ) AND( \"NAME\" IS NOT NULL ))", new List<SugarParameter>()
            {

            }, "whereSingle14");
        }

        private void whereSingle15()
        {
            Expression<Func<DataTestInfo, bool>> exp = it =>it.Money2 == 1;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"MONEY2\" = :Const0 )", new List<SugarParameter>()
            {
                new SugarParameter(":Const0",1)
            }, "whereSingle15");
        }

        private void whereSingle16()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("key", "x1");
            Expression<Func<DataTestInfo, bool>> exp = it => it.String == dic["key"];
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"String\" = :Const0 )", new List<SugarParameter>()
            {
                new SugarParameter(":Const0",dic["key"])
            }, "whereSingle16");
        }

        private void whereSingle17()
        {
            Expression<Func<DataTestInfo, bool>> exp = it =>true&&it.String.Contains("a");
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( 1 = 1 ) AND (\"STRING\" like '%'+:MethodConst1+'%') )", new List<SugarParameter>()
            {
                new SugarParameter(":MethodConst1","a")
            }, "whereSingle17");
        }

        private void whereSingle18()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => !it.Bool1;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "NOT ( \"BOOL1\"=1 )  ", new List<SugarParameter>()
            {
   
            }, "whereSingle18");
        }
        private void whereSingle19()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => it.Bool2.Value==false;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"BOOL2\" = :Value0 )", new List<SugarParameter>()
            {
                new SugarParameter(":Value0",false)
            }, "whereSingle19");
        }
        private void whereSingle20()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => it.Bool2.Value == it.Bool1;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"BOOL2\" = \"BOOL1\" )", new List<SugarParameter>()
            {
                
            }, "whereSingle19");
        }

        private void whereSingle21()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => it.Bool2.Value;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"BOOL2\"=1 )", new List<SugarParameter>()
            {

            }, "whereSingle21");
        }

        private void whereSingle22()
        {
            Expression<Func<DataTestInfo2, bool>> exp = it => !it.Bool2.Value;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "NOT ( \"BOOL2\"=1 ) ", new List<SugarParameter>()
            {

            }, "whereSingle22");
        }

        private void whereSingle23()
        {
            decimal? val = 1;
            Expression<Func<DataTestInfo, bool>> exp = it => it.Decimal2==val.Value;
            OracleExpressionContext expContext = new OracleExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( \"DECIMAL2\" = :Const0 )", new List<SugarParameter>()
            {
                new SugarParameter(":Const0",val)
            }, "whereSingle23");
        }
    }

    public class WhereConst
    {
        public static string name { get; set; }
    }
}
