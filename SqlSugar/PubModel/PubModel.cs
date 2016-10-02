using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：公共参数表
    /// ** 创始时间：2015-7-20
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class PubModel
    {
        /// <summary>
        /// 用于存储数据表与列的映射信息
        /// </summary>
        public class DataTableMap
        {
            /// <summary>
            /// 表名
            /// </summary>
            public object TABLE_NAME { get; set; }
            /// <summary>
            /// 表ID
            /// </summary>
            public object TABLE_ID { get; set; }
            /// <summary>
            /// 列名
            /// </summary>
            public object COLUMN_NAME { get; set; }
            /// <summary>
            /// 数据类型
            /// </summary>
            public object DATA_TYPE { get; set; }
            /// <summary>
            /// 字符最大长度
            /// </summary>
            public object CHARACTER_MAXIMUM_LENGTH { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public object COLUMN_DESCRIPTION { get; set; }
            /// <summary>
            /// 默认值
            /// </summary>
            public object COLUMN_DEFAULT { get; set; }
            /// <summary>
            /// 是否允许为null
            /// </summary>
            public object IS_NULLABLE { get; set; }
            /// <summary>
            /// 是否是主键
            /// </summary>
            public object IS_PRIMARYKEY { get; set; }
        }

        /// <summary>
        /// 流水号设置实体
        /// </summary>
        public class SerialNumber
        {
            /// <summary>
            /// 表名
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 字段名
            /// </summary>
            public string FieldName { get; set; }
            /// <summary>
            /// 获取流水号函数
            /// </summary>
            public Func<string> GetNumFunc { get; set; }
            /// <summary>
            /// 获取流水号函数(解决事务中死锁BUG)
            /// </summary>
            public Func<SqlSugarClient,string> GetNumFuncWithDb { get; set; }
        }

        /// <summary>
        /// 多语言设置的参数表
        /// </summary>
        public class Language
        {
            /// <summary>
            /// 数据库里面的语言后缀
            /// </summary>
            public string Suffix { get; set; }
            /// <summary>
            /// 数据库语言的VALUE
            /// </summary>
            public int LanguageValue { get; set; }
            /// <summary>
            /// 需要全局替换的字符串Key(用于替换默认语言)
            /// </summary>
            public string ReplaceViewStringKey = "LanguageId=1";
            /// <summary>
            /// 需要全局替换的字符串Value(用于替换默认语言)
            /// </summary>
            public string ReplaceViewStringValue = "LanguageId = {0}";

        }
        /// <summary>
        /// SqlSugarClient通用常量
        /// </summary>
        internal class SqlSugarClientConst {
            /// <summary>
            /// 属性设置错误信息
            /// </summary>
            public const string AttrMappingError = @"[SugarMapping(ColumnName = ""{1}"")]
  public string {0} {{ get; set; }}已经在其存在于其它表， Columns映射只能在 {0}->{1}和{0}->{2}二者选其一";
            /// <summary>
            /// SqlQuery查询的SQL模板
            /// </summary>
            public const string SqlQuerySqlTemplate = @"--{0}
{1}";
        }
    }
}
