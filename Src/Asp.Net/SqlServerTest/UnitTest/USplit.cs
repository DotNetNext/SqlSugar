using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public partial class NewUnitTest {
        public static void SplitTest() 
        {
            var db = Db;

            db.CodeFirst.SplitTables().InitTables<SplitTestTable>();

            var data = new SplitTestTable()
            {
                 CreateTime=Convert.ToDateTime("2019-12-1"),
                 Name="jack"
            };
            var datas = new List<SplitTestTable>()
            {
                new SplitTestTable()
               {
                 CreateTime=Convert.ToDateTime("2019-12-1"),
                   Name="jack"
                } ,
                new SplitTestTable()
               {
                 CreateTime=Convert.ToDateTime("2022-02-1"),
                   Name="jack"
                },
                new SplitTestTable()
               {
                 CreateTime=Convert.ToDateTime("2020-02-1"),
                   Name="jack"
                },
                new SplitTestTable()
               {
                 CreateTime=Convert.ToDateTime("2021-12-1"),
                   Name="jack"
                }
            };

            long id= db.Insertable(data).SplitTable().ExecuteReturnSnowflakeId();

            List<long> ids= db.Insertable(datas).SplitTable().ExecuteReturnSnowflakeIdList();

            var count = 0;
            db.Queryable<SplitTestTable>().Where(it => it.Name.Contains("a")).SplitTable(tas => tas.Take(3)).ToPageList(1,2,ref count);

            var table2019=Db.SplitHelper<SplitTestTable>().GetTableName("2019-12-1");
            db.Queryable<SplitTestTable>().Where(it => it.Name.Contains("a")).SplitTable(tas => tas.InTableNames(table2019)).ToList();

            db.Queryable<SplitTestTable>().Where(it => it.Id == 1).SplitTable(tas => tas.Where(y => y.TableName.Contains("2019"))).ToList();

            var deldata = new SplitTestTable()
            {
                Id = id,
                CreateTime = DateTime.Now
            };
            var tableName = db.SplitHelper(deldata).GetTableNames();
            db.Deleteable<SplitTestTable>().Where(deldata).SplitTable(tas=>tas.InTableNames(tableName)).ExecuteCommand();


            db.Updateable(deldata).SplitTable(tas=>tas.Take(3)).ExecuteCommand();
            db.Updateable(deldata).SplitTable(tas => tas.InTableNames(tableName)).ExecuteCommand();


            //使用自定义分表
            db.CurrentConnectionConfig.ConfigureExternalServices.SplitTableService =new WordSplitService();
    db.Insertable(new WordTestTable()
    {
            CreateTime=DateTime.Now,
            Name="BC"
    }).SplitTable().ExecuteReturnSnowflakeId();
    db.Insertable(new WordTestTable()
    {
        CreateTime = DateTime.Now,
        Name = "AC"
    }).SplitTable().ExecuteReturnSnowflakeId();
    db.Insertable(new WordTestTable()
    {
        CreateTime = DateTime.Now,
        Name = "ZBZ"
    }).SplitTable().ExecuteReturnSnowflakeId();

            //只查A表
            var tableName2 = db.SplitHelper(new WordTestTable() {  Name="A" }).GetTableNames();
            var listall1 = db.Queryable<WordTestTable>().Where(it => it.Name == "all").SplitTable(tas => tas.InTableNames(tableName2)).ToList();
            var listall = db.Queryable<WordTestTable>().Where(it => it.Name == "all").SplitTable(tas => tas.ContainsTableNames("_FirstA")).ToList();
         
        }
    }



    /// <summary>
    /// 随便设置一个分类
    /// </summary>
    [SplitTable(SplitType._Custom01)]
    public class WordTestTable 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }

        [SplitField] //标识一下分表字段
        public string Name { get; set; }

   
        public DateTime CreateTime { get; set; }

    }

    public class WordSplitService : ISplitTableService
    {
        /// <summary>
        /// 返回数据用于 tas 进行筛选
        /// </summary>
        /// <param name="db"></param>
        /// <param name="EntityInfo"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in tableInfos)
            {
                if (item.Name.Contains(EntityInfo.DbTableName+"_First")) //区分标识如果不用正则符复杂一些，防止找错表
                {
                    SplitTableInfo data = new SplitTableInfo()
                    {
                        TableName = item.Name //要用item.name不要写错了
                    };
                    result.Add(data);
                }
            }
            return result.OrderBy(it=>it.TableName).ToList();
        }

        /// <summary>
        /// 获取分表字段的值
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entityInfo"></param>
        /// <param name="splitType"></param>
        /// <param name="entityValue"></param>
        /// <returns></returns>
        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
            return value;
        }
        /// <summary>
        /// 默认表名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="EntityInfo"></param>
        /// <returns></returns>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo)
        {
            return entityInfo.DbTableName + "_FirstA"; 
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType type)
        {
            return entityInfo.DbTableName + "_FirstA";//目前模式少不需要分类(自带的有 日、周、月、季、年等进行区分)
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            return entityInfo.DbTableName + "_First" + GetFirstCode(fieldValue+""); //根据值按首字母
        }

        #region 获取首字母
        /// <summary>
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串
        /// </summary>
        /// <param name="CnStr">汉字字符串</param>
        /// <returns>相对应的汉语拼音首字母串</returns>
        public static string GetFirstCode(string CnStr)
        {
            string Surname = CnStr.Substring(0, 1);
            string strTemp = GetSpellCode(Surname);
            return strTemp;
        }

        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        private static string GetSpellCode(string CnChar)
        {
            long iCnChar;
            byte[] arrCN = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回
            if (arrCN.Length == 1)
            {
                CnChar = CnChar.ToUpper();
            }
            else
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                iCnChar = (area << 8) + pos;

                // iCnChar match the constant
                string letter = "ABCDEFGHJKLMNOPQRSTWXYZ";
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52980, 53689, 54481, 55290 };
                for (int i = 0; i < 23; i++)
                {
                    if (areacode[i] <= iCnChar && iCnChar < areacode[i + 1])
                    {
                        CnChar = letter.Substring(i, 1);
                        break;
                    }
                }
            }
            return CnChar;
        }
        #endregion
    }


    [SplitTable(SplitType.Year)]//按年分表
    [SugarTable("SplitTestTable_{year}{month}{day}")]//生成表名格式 3个变量必须要有
    public class SplitTestTable 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }

        public string Name { get; set; }
        [SplitField]
        public DateTime CreateTime { get; set; }
    }
}
