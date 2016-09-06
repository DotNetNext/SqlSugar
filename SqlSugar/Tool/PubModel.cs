using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class PubModel
    {
        #region 表字段映射实体

        public class DataTableMap
        {
            public object TABLE_NAME { get; set; }

            public object TABLE_ID { get; set; }

            public object COLUMN_NAME { get; set; }

            public object DATA_TYPE { get; set; }

            public object CHARACTER_MAXIMUM_LENGTH { get; set; }

            public object COLUMN_DESCRIPTION { get; set; }

            public object COLUMN_DEFAULT { get; set; }

            public object IS_NULLABLE { get; set; }
        }

        #endregion

        /// <summary>
        /// 流水号设置实体
        /// </summary>
        public class SerialNumber
        {
            public string TableName { get; set; }
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
        /// 语言
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
    }
}
