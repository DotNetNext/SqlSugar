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
                IIF();
                IIF2();
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

        private void ExtendToString()
        {
            Expression<Func<Student, bool>> exp = it => it.Id.ToString() == "a";
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(`Id` AS CHAR) = @Const0 )", new List<SugarParameter>() {
                 new SugarParameter("@Const0","a")
            }, "ExtendToString error");
        }

        private void ConvetToString()
        {
            Expression<Func<Student, bool>> exp = it => Convert.ToString(it.Id) == "a";
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(`Id` AS CHAR) = @Const0 )", new List<SugarParameter>() {
                 new SugarParameter("@Const0","a")
            }, "ConvetToString error");
        }


        private void Length()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Length("aaaa") > 1;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(LENGTH(@MethodConst0) > @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","aaaa"),new SugarParameter("@Const1",1)
            }, "Length error");
        }

        private void Replace()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.Replace("aaaa", "a", "1") == "a";
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS SIGNED) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","true"),new SugarParameter("@Const1",(bool)true)
            }, "ToBool error");
        }

        private void ToDouble()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDouble("2") == 2;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS DECIMAL(18,4)) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2"),new SugarParameter("@Const1",(Double)2)
            }, "ToDouble error");
        }

        private void ToGuid()
        {
            var x2 = Guid.NewGuid();
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToGuid("A94027A3-476E-478D-8228-F4054394B874") == x2;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS CHAR) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","A94027A3-476E-478D-8228-F4054394B874"),new SugarParameter("@Const1",x2)
            }, "ToGuid error");
        }

        private void ToDecimal()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDecimal("22") == 1;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS DECIMAL(18,4)) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","22"),new SugarParameter("@Const1",(decimal)1)
            }, "ToDecimal error");
        }

        private void Tostring()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToString("2015-1-1") == "a";
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS CHAR) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","2015-1-1"),new SugarParameter("@Const1","a")
            }, "Tostring error");
        }

        private void ToDate()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToDate("2015-1-1") == x2;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS SIGNED) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","3"),new SugarParameter("@Const1",(Int64)1)
            }, "ToInt64 error");
        }

        private void ToInt32()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.ToInt32("3") == 1;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(CAST(@MethodConst0 AS SIGNED) = @Const1 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","3"),new SugarParameter("@Const1",1)
            }, "ToInt32 error");
        }

        private void DateValue()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateValue(x2, DateType.Year) == 1;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (Year(@MethodConst0) = @Const2 ) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@Const2",1)
            }, "DateValue error");
        }

        private void StartsWith()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.StartsWith(it.Name, "a");
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` like concat(@MethodConst0,'%')) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "StartsWith error");
        }
        private void EndsWith()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.EndsWith(it.Name, "a");
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` like concat('%',@MethodConst0)) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "EndsWith");
        }
        private void Between()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Between(it.Name, 1, 2);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` BETWEEN @MethodConst0 AND @MethodConst1) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",1),new SugarParameter("@MethodConst1",2),
            }, "Between error");
        }

        private void DateAddByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateAdd(x2, 11, DateType.Millisecond) == x2;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATE_ADD(@MethodConst0 , INTERVAL @MethodConst1 Millisecond)) = @Const3 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",11),new SugarParameter("@Const3",x2)
            }, "DateAddByType error");
        }
        private void DateAddDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateAdd(x2, 1) == x2;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((DATE_ADD(@MethodConst1 INTERVAL @MethodConst0 day)) = @Const2 )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",1),new SugarParameter("@Const2",x2)
            }, "DateIsSameByType error");
        }

        private void DateIsSameByType()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateIsSame(x2, x2, DateType.Millisecond);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (TIMESTAMPDIFF(Millisecond,@MethodConst0,@MethodConst1)=0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",x2)
            }, "DateIsSameByType error");
        }
        private void DateIsSameByDay()
        {
            var x2 = DateTime.Now;
            Expression<Func<Student, bool>> exp = it => SqlFunc.DateIsSame(x2, x2);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (TIMESTAMPDIFF(day,date(@MethodConst0),date(@MethodConst1))=0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0",x2),new SugarParameter("@MethodConst1",x2)
            }, "DateIsSameDay error");
        }

        private void Equals()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Equals(it.Name, "a");
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` = @MethodConst0) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => SqlFunc.Equals("a", it.Name);
            MySqlExpressionContext expContext2 = new MySqlExpressionContext();
            expContext2.Resolve(exp2, ResolveExpressType.WhereSingle);
            var value2 = expContext2.Result.GetString();
            var pars2 = expContext2.Parameters;
            base.Check(value2, pars2, " (@MethodConst0 = `Name`) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals2 error");
        }
        private void Equals_2()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Equals(it.Name, it.Name);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` = `Name`) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Equals1 error");


            Expression<Func<Student, bool>> exp2 = it => SqlFunc.Equals("a", "a2");
            MySqlExpressionContext expContext2 = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` like concat('%',@MethodConst0,'%')) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains error");
        }
        private void Contains2(string name="a")
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Contains(it.Name, name);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, " (`Name` like concat('%',@MethodConst0,'%')) ", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","a")
            }, "Contains2 error");
        }

        private void ExtendContainsArray() {
            var array = new string[] { "1", "2" }.ToList();
            Expression<Func<Student, bool>> exp = it => array.Contains(it.Name);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, null, "  (`Name` IN ('1','2')) ", null, "Contains2 error");
        }

        private void ContainsArray()
        {
            string[] array = new string[] { "1", "2" };
            Expression<Func<Student, bool>> exp = it => SqlFunc.ContainsArray(array, it.Name);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, null, "  (`Name` IN ('1','2')) ", null, "Contains2 error");
        }

        private void Trim()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.Trim("  a") == it.Name;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "((rtrim(ltrim(@MethodConst0))) = `Name` )", new List<SugarParameter>() {
                new SugarParameter("@MethodConst0","  a")
            }, "Trim error");
        }

        private void ToUpper()
        {
            Expression<Func<Student, bool>> exp = it => "a" == SqlFunc.ToUpper(it.Id);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0 = (UPPER(`Id`)) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToUpper error");
        }
        private void ToLower()
        {
            Expression<Func<Student, bool>> exp = it => "a" == SqlFunc.ToLower(it.Id);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0 = (LOWER(`Id`)) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToLower error");
        }

        #region StringIsNullOrEmpty
        private void StringIsNullOrEmpty()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 2 || SqlFunc.IsNullOrEmpty(it.Id); ;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( `Id` > @Id0 ) OR ( `Id`='' OR `Id` IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty error");
        }
        private void StringIsNullOrEmpty2()
        {
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(true); ;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = `Id` ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",true),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty2 error");
        }
        private void StringIsNullOrEmpty3()
        {
            int a = 1;
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(a); ;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = `Id` ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",1),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty3 error");
        }
        private void StringIsNullOrEmpty4()
        {
            WhereConst.name = "xx";
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || SqlFunc.IsNullOrEmpty(WhereConst.name); ;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0 = `Id` ) OR ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1","xx"),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty4 error");
        }
        private void StringIsNullOrEmpty5()
        {
            WhereConst.name = "xx";
            Expression<Func<Student, bool>> exp = it => !SqlFunc.IsNullOrEmpty(WhereConst.name); ;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( `Name`<>'' AND `Name` IS NOT NULL )", new List<SugarParameter>()
            {

            }, "HasValue error");
        }

        private void HasNumber()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.HasNumber(it.Id);
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( `Id`>0 AND `Id` IS NOT NULL )", new List<SugarParameter>()
            {


            }, "HasNumber error");
        }

        private void IIF()
        {
            Expression<Func<Student, bool>> exp = it => SqlFunc.IIF(it.Id == 1, 1, 2)==1;
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN ( `Id` = @Id0 ) THEN @MethodConst1  ELSE @MethodConst2 END ) = @Const3 )", new List<SugarParameter>()
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
            MySqlExpressionContext expContext = new MySqlExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( CASE  WHEN  (`Name` like concat('%',@MethodConst0,'%'))  THEN @MethodConst1  ELSE @MethodConst2 END ) = @Const3 )", new List<SugarParameter>()
            {
                     new SugarParameter("@MethodConst0","a"),
                     new SugarParameter("@MethodConst1",1),
                     new SugarParameter("@MethodConst2",2),
                                     new SugarParameter("@Const3",1)
            }, "IIF2 error");
        }
    }
}

