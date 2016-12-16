using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：多语言视图帮助类
    /// ** 创始时间：2016-8-7
    /// ** 修改时间：-
    /// ** 作者：孙凯旋
    /// ** 使用说明：
    /// </summary>
    public class LanguageHelper
    {
        /// <summary>
        /// 多语言视图的前缀
        /// </summary>
        public static string PreSuffix = "_$_";
        /// <summary>
        /// 获取所有需要生成多语言的视图名称
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<string> GetLanguageViewNameList(SqlSugarClient db)
        {
            string key = "LanguageHelper.GetViewNameList";
            var cm = CacheManager<List<string>>.GetInstance();
            if (cm.ContainsKey(key))
            {
                return cm[key];
            }
            else
            {
                var list = db.SqlQuery<string>(@"
	            select a.name from sys.objects a 
	            JOIN sys.sql_modules b on a.[object_id]=b.[object_id]
	            where [type]='v' 
		        and b.[definition] like '%" + db.Language.ReplaceViewStringKey + @"%'
		        and a.name not like '%"+PreSuffix+@"%'
                ").ToList();
                cm.Add(key, list, cm.Day);
                return list;
            }

        }

        /// <summary>
        /// 创建多语言视图，带有LanguageId=1的所有有视图1替换成languageValue 并且新创视图 名称为 原有视图名+_$_+suffix
        /// </summary>
        /// <returns></returns>
        public static void UpdateView(PubModel.Language lan, SqlSugarClient db)
        {
            if (lan == null) return;
            if (lan.Suffix.IsNullOrEmpty())
            {
                Check.Exception(true, "LanguageHelper.lan.Suffix is Null Or Empty");
            }
            if (PreSuffix.IsNullOrEmpty())
            {
                Check.Exception(true, "LanguageHelper.PreSuffix is Null Or Empty");
            }


            if (!lan.Suffix.StartsWith(PreSuffix))
            {
                lan.Suffix = PreSuffix + lan.Suffix;
            }

            string sql = @"

	                        --验证参数传递规则
	                        if LEFT(ltrim(@Suffix),3)<>'" + PreSuffix + @"'
	                        begin
		                        raiserror('参数传递格式不规范',16,1)
		                        return;
	                        end
	                        else
	                        if(ISNULL("+lan.LanguageValue+@",'')='')
	                        begin
		                        raiserror('参数传递格式不规范',16,1)
		                        return;
	                        end
	
	                        declare 
			                        @name		varchar(100),	--视图名称
			                        @definition varchar(max)	--视图脚本
	                        --删除数据库里面所有带传递参数几号的视图
	                        declare my_cursor cursor for
	                        select a.name,b.[definition] from sys.objects a 
	                        JOIN sys.sql_modules b on a.[object_id]=b.[object_id]
	                        where [type]='v' 
		                          and b.[definition] like '%" + lan.ReplaceViewStringKey + @"%'
		                          and a.name not like '%"+PreSuffix+@"%'
	                        --打开处理器
	                        open my_cursor
	                        fetch next from my_cursor into @name,@definition
	                        while @@FETCH_STATUS=0
	                        begin
		                        --脚本查询语言ID更改,并且更改新脚本语言的对象名称
		                        set	@definition=REPLACE(
								                        REPLACE(
										                         @definition,
										                         '" + lan.ReplaceViewStringKey + @"',
										                         '" + string.Format(lan.ReplaceViewStringValue, lan.LanguageValue) + @"'
									                           ),
								                        @name,
								                        @name+@Suffix
								                        )
		                        --判断新脚本语言的对象名称是否存在,存在删除
		                        exec(
			                        '
				                        if object_id('''+@name+@Suffix+''',''v'') is not null
				                        begin
					                        drop view '+@name+@Suffix+'
				                        end
			
			                        '
		                        )
		                        exec(@definition)
	                        fetch next from my_cursor into @name,@definition
	                        end
	                        close my_cursor
	                        deallocate my_cursor
";

            db.ExecuteCommand(sql, new { Suffix = lan.Suffix });

        }
    }
}
