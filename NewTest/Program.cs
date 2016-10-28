using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using NewTest.Demos;
namespace NewTest
{
    class Program
    {
        /// <summary>
        /// SqlSugar的功能介绍
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //设置执行的DEMO
            string switchOn = "select";
            IDemos demo = null;
            switch (switchOn)
            {
                /****************************基本功能**************************************/
                //查询
                case "select": demo = new Select(); break;
                //删除
                case "delete": demo = new Delete(); break;
                //插入
                case "insert": demo = new Insert(); break;
                //更新
                case "update": demo = new Update(); break;
                //基层函数的用法
                case "ado": demo = new Ado(); break;
                //事务
                case "tran": demo = new Tran(); break;
                //创建实体函数
                case "createclass": demo = new CreateClass(); break;
                //T4生成 http://www.cnblogs.com/sunkaixuan/p/5751503.html

                //日志记录
                case "log": demo = new Log(); break;
                //枚举支持
                case "enum": demo = new EnumDemo(); break;


                
                /****************************实体映射**************************************/
                //自动排除非数据库列
                case "ignoreerrorcolumns": demo = new IgnoreErrorColumns(); break;
                //别名表
                case "mappingtable":demo=new MappingTable(); break;
                //别名列
                case "mappingcolumns": demo = new MappingColumns(); break;
                //通过属性的方法设置别名表和别名字段
                case "attributesmapping": demo = new AttributesMapping(); break;



                /****************************业务应用**************************************/
                //过滤器
                case "filter": demo = new Filter(); break;
                //过滤器2
                case "filter2": demo = new Filter2(); break;
                //流水号功能
                case "serialnumber": demo = new SerialNumber(); break;

                //多语言支持 http://www.cnblogs.com/sunkaixuan/p/5709583.html
                //多库并行计算 http://www.cnblogs.com/sunkaixuan/p/5046517.html

                //配置与实例的用法
                case "initconfig": demo = new InitConfig(); break;



                /****************************支持**************************************/
                //公开函数数
                case "pubmethod": demo = new PubMethod(); break;
                //Sql2012分页的支持
                case "sqlpagemodel": demo=new SqlPageModel(); break;
                //设置ToJson的日期格式
                case "serializerdateformat":demo =new SerializerDateFormat();break;

    

                /****************************测试用例**************************************/
                case "test": demo = new Test(); break;

                default: Console.WriteLine("switchOn的值错误，请输入正确的 case"); break;

            }
            //执行DEMO
            demo.Init();

            //更多例子请查看API
            //http://www.cnblogs.com/sunkaixuan/p/5654695.html
            Console.WriteLine("执行成功请关闭窗口");
            Console.ReadKey();
        }
    }
}
