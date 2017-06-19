using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    //局部类：拉姆达解析公用常量
    internal partial class ResolveExpress
    {
        /// <summary>
        /// 解析bool类型用到的字典
        /// </summary>
        public static List<ExpressBoolModel> ConstantBoolDictionary = new List<ExpressBoolModel>()
        {
               new ExpressBoolModel(){ Key=Guid.NewGuid(), OldValue="True", Type=SqlSugarTool.StringType},
               new ExpressBoolModel(){ Key=Guid.NewGuid(), OldValue="False",Type=SqlSugarTool.StringType},
               new ExpressBoolModel(){ Key=Guid.NewGuid(), OldValue="True",Type=SqlSugarTool.BoolType},
               new ExpressBoolModel(){ Key=Guid.NewGuid(), OldValue="False",Type=SqlSugarTool.BoolType}

        };
        /// <summary>
        /// 字段名解析错误
        /// </summary>
        public const string FileldErrorMessage = "OrderBy、GroupBy、In、Min和Max等操作不是有效拉姆达格式 ，正确格式 it=>it.name 。";
        /// <summary>
        /// 拉姆达解析错误
        /// </summary>
        public const string ExpToSqlError= @"拉姆达解析出错，不是有效的函数，找不到合适函数你可以使用这种字符串写法.Where(""date>dateadd(@date)"",new{date=DateTime.Now})，
                支持的函数有(请复制到本地查看，数量比较多):
                db.Queryable<T>().Where(it => it.field == parValue.ObjToString()); 
                db.Queryable<T>().Where(it => it.field == parValue.ObjToDate());
                db.Queryable<T>().Where(it => it.field == parValue.ObjToInt())
                db.Queryable<T>().Where(it => it.field == parValue.ObjToDecimal())
                db.Queryable<T>().Where(it => it.field == parValue.ObjToMoney())
                db.Queryable<T>().Where(it => it.field == parValue.Trim())
                db.Queryable<T>().Where(it => it.field == parValue.ToString())
                db.Queryable<T>().Where(it => it.field == Convert.ToXXX(parValue))
                db.Queryable<T>().Where(it => it.field.Contains(parValue))
                db.Queryable<T>().Where(it => it.field.StartsWith(parValue))
                db.Queryable<T>().Where(it => it.field.EndsWith(parValue))
                db.Queryable<T>().Where(it => !string.IsNullOrEmpty(it.parValue))
                db.Queryable<T>().Where(it => arrayOrList.Contains(it.parValue))     
                db.Queryable<T>().Where(it => it.field.Equals(it.parValue))  
                db.Queryable<T>().Where(it => it.field.Length>10)
                db.Queryable<Student>().Where(c => c.field == parValue.ToLower()).ToList();
                db.Queryable<Student>().Where(c => c.field == parValue.ToUpper()).ToList();       
            ";
        /// <summary>
        /// 运算符错误
        /// </summary>
        public const string OperatorError = "拉姆达解析出错：不支持{0}此种运算符查找！";

        /// <summary>
        /// 拉姆达解析唯一标识
        /// </summary>
        public static object ExpErrorUniqueKey = Guid.NewGuid();

        /// <summary>
        /// 拉姆达函数错误
        /// </summary>
        public const string ExpMethodError = "拉姆达表达式中的函数用法不正确，正确写法 it=>it.name.{0}(参数) ,不支持的写法it=> 参数.{0}(it.name)。";

        /// <summary>
        /// 拉姆达函数错误2
        /// </summary>
        public const string ExpMethodError2 = "拉姆达表达式中的函数用法不正确，正确写法 it=>it.name==参数.{0} ,不支持的写法it=>it.name.{0}==参数。";

        /// <summary>
        /// 表达式不支持解析BlockExpression
        /// </summary>
        public const string ExpBlockExpression = "表达式不支持解析BlockExpression,错误信息:";

        /// <summary>
        /// 表达式不支解析ConditionalExpression
        /// </summary>
        public const string ExpConditionalExpression = "表达式不支解析ConditionalExpression,错误信息:";

        /// <summary>
        /// 拉姆达表达式内不支持new对象
        /// </summary>
        public const string ExpNew = "拉姆达表达式内不支持new对象，请提取变量后在赋值，错误信息:";

        /// <summary>
        /// 不支持对象或属性
        /// </summary>
        public const string ExpNoSupportObjectOrAttr = "拉姆达解析不支持{0}对象，或该{1}的属性。";

        /// <summary>
        /// 不支持属性扩展方法
        /// </summary>
        public const string ExpNoSupportAttExtMethod = "不支持属性扩展方法{0}。";
    }


     //局部类:Select表达式对象解析公用常量
    internal partial class ResolveSelect {

        /// <summary>
        /// 解析对象不能为null
        /// </summary>
        public const string ExpSelectValueIsNull = "解析对象不能为null。";

        /// <summary>
        ///Select中不支持函数
        /// </summary>
        public const string ExpNoSupportMethod = "Select中不支持函数{0}。";

        /// <summary>
        /// Select中不支持变量的运算
        /// </summary>
        public const string ExpNoSupportOperation = "Select中不支持变量的运算。";

        /// <summary>
        /// 不支持外部传参
        /// </summary>
        public static string ExpNoSupportOutPars = "Select中的拉姆达表达式,不支持外部传参数,目前支持的写法: Where(\"1=1\",new {id=1}).Select(it=>{ id=\"" + SqlSugarTool.ParSymbol + "id\".ObjToInt()} 。";

        /// <summary>
        /// 不支持ToString
        /// </summary>
        public static string ExpNoSupportToString = "Select中不支持ToString函数，请使用ObjectToString。";
    }
}
