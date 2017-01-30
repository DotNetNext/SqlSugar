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
                #endregion
            }
            base.End("Method Test");
        }

        private void ToUpper()
        {
            Expression<Func<Student, bool>> exp = it =>"a"== NBORM.ToUpper(it.Id) ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0  = (UPPER(Id)) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToUpper");
        }
        private void ToLower()
        {
            Expression<Func<Student, bool>> exp = it => "a" == NBORM.ToLower(it.Id);
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "( @Const0  = (LOWER(Id)) )", new List<SugarParameter>() {
                new SugarParameter("@Const0","a")
            }, "ToLower");
        }

        #region StringIsNullOrEmpty
        private void StringIsNullOrEmpty()
        {
            Expression<Func<Student, bool>> exp = it => it.Id > 2 || NBORM.IsNullOrEmpty(it.Id); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( Id  > @Id0 )  OR  ( Id='' OR Id IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty");
        }
        private void StringIsNullOrEmpty2()
        {
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(true); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0  = Id )  OR  ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",true),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty2");
        }
        private void StringIsNullOrEmpty3()
        {
            int a = 1;
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(a); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0  = Id )  OR  ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1",1),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty3");
        }
        private void StringIsNullOrEmpty4()
        {
            WhereConst.name = "xx";
            Expression<Func<Student, bool>> exp = it => 2 == it.Id || NBORM.IsNullOrEmpty(WhereConst.name); ;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( @Id0  = Id )  OR  ( @MethodConst1='' OR @MethodConst1 IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@MethodConst1","xx"),
                new SugarParameter("@Id0",2)
            }, "StringIsNullOrEmpty4");
        } 
        #endregion
    }
}

