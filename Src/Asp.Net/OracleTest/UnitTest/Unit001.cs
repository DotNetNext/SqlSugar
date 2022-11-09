 using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlSugar;
using static OrmTest.Program;

namespace OrmTest
{
    public class Unit001
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
 

            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响


                //5.0.8.2 获取无参数化 SQL  对性能有影响，特别大的SQL参数多的，调试使用
                //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
            };
            db.CodeFirst.InitTables<SfbaT, ImaalT, InagT>();
            var result = db.Queryable<SfbaT>()
                .LeftJoin<ImaalT>((a, l) => a.Sfbaent == l.Imaalent && a.Sfba006 == l.Imaal001 && l.Imaal002 == "zh_CH")
                .Where(a => a.Sfbadocno == "AAA" && a.Sfba008 != "3")
                .Select(a => new
                {

                    WarehouseLocation = SqlFunc.Subqueryable<InagT>()
                                .Where(g => g.Inag001 == a.Sfba006)
                                .Where(g => (new string[] { "40", "41", "42", "49" }).Contains(g.Inag004.ToString()))
                                .Where(g => g.Inagent == a.Sfbaent && g.Inagsite == a.Sfbasite)
                                .Where(g => SqlFunc.IsNull(g.Inag008, 0) > 0)
                                .SelectStringJoin(g => SqlFunc.MergeString(g.Inag004, "-", g.Inag008.ToString()), ",")
                }).ToList();
 
        }
        //建类
        public class Test001
        {
            public int id { get; set; }
        }
        /// <summary>
        /// 工单备料单身档
        ///</summary>
        [SugarTable("SFBA_T")]
        public class SfbaT
        {
            /// <summary>
            /// 企业编号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAENT", IsPrimaryKey = true)]
            public string Sfbaent { get; set; }
            /// <summary>
            /// 营运据点 
            ///</summary>
            [SugarColumn(ColumnName = "SFBASITE")]
            public string Sfbasite { get; set; }
            /// <summary>
            /// 单号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBADOCNO", IsPrimaryKey = true)]
            public string Sfbadocno { get; set; }
            /// <summary>
            /// 项次 
            ///</summary>
            [SugarColumn(ColumnName = "SFBASEQ", IsPrimaryKey = true)]
            public long Sfbaseq { get; set; }
            /// <summary>
            /// 项序 
            ///</summary>
            [SugarColumn(ColumnName = "SFBASEQ1", IsPrimaryKey = true)]
            public long Sfbaseq1 { get; set; }
            /// <summary>
            /// 上阶料号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA001")]
            public string Sfba001 { get; set; }
            /// <summary>
            /// 部位 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA002")]
            public string Sfba002 { get; set; }
            /// <summary>
            /// 作业编号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA003")]
            public string Sfba003 { get; set; }
            /// <summary>
            /// 作业序 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA004")]
            public string Sfba004 { get; set; }
            /// <summary>
            /// BOM料号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA005")]
            public string Sfba005 { get; set; }
            /// <summary>
            /// 发料料号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA006")]
            public string Sfba006 { get; set; }
            /// <summary>
            /// 投料时距 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA007")]
            public string Sfba007 { get; set; }
            /// <summary>
            /// 必要特性 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA008")]
            public string Sfba008 { get; set; }
            /// <summary>
            /// 倒扣料 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA009")]
            public string Sfba009 { get; set; }
            /// <summary>
            /// 标准QPA分子 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA010")]
            public decimal? Sfba010 { get; set; }
            /// <summary>
            /// 标准QPA分母 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA011")]
            public decimal? Sfba011 { get; set; }
            /// <summary>
            /// 允许误差率 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA012")]
            public decimal? Sfba012 { get; set; }
            /// <summary>
            /// 应发数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA013")]
            public decimal? Sfba013 { get; set; }
            /// <summary>
            /// 单位 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA014")]
            public string Sfba014 { get; set; }
            /// <summary>
            /// 委外代买数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA015")]
            public decimal? Sfba015 { get; set; }
            /// <summary>
            /// 已发数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA016")]
            public decimal? Sfba016 { get; set; }
            /// <summary>
            /// 报废数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA017")]
            public decimal? Sfba017 { get; set; }
            /// <summary>
            /// 盘盈亏数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA018")]
            public decimal? Sfba018 { get; set; }
            /// <summary>
            /// 指定发料仓库 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA019")]
            public string Sfba019 { get; set; }
            /// <summary>
            /// 指定发料储位 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA020")]
            public string Sfba020 { get; set; }
            /// <summary>
            /// 产品特征 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA021")]
            public string Sfba021 { get; set; }
            /// <summary>
            /// 替代率 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA022")]
            public decimal? Sfba022 { get; set; }
            /// <summary>
            /// 标准应发数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA023")]
            public decimal? Sfba023 { get; set; }
            /// <summary>
            /// 调整应发数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA024")]
            public decimal? Sfba024 { get; set; }
            /// <summary>
            /// 超领数量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA025")]
            public decimal? Sfba025 { get; set; }
            /// <summary>
            /// SET替代状态 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA026")]
            public string Sfba026 { get; set; }
            /// <summary>
            /// SET替代群组 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA027")]
            public string Sfba027 { get; set; }
            /// <summary>
            /// 客供料 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA028")]
            public string Sfba028 { get; set; }
            /// <summary>
            /// 指定发料批号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA029")]
            public string Sfba029 { get; set; }
            /// <summary>
            /// 指定库存管理特征 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA030")]
            public string Sfba030 { get; set; }
            /// <summary>
            /// 自定义字段(文本)001 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD001")]
            public string Sfbaud001 { get; set; }
            /// <summary>
            /// 自定义字段(文本)002 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD002")]
            public string Sfbaud002 { get; set; }
            /// <summary>
            /// 自定义字段(文本)003 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD003")]
            public string Sfbaud003 { get; set; }
            /// <summary>
            /// 自定义字段(文本)004 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD004")]
            public string Sfbaud004 { get; set; }
            /// <summary>
            /// 自定义字段(文本)005 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD005")]
            public string Sfbaud005 { get; set; }
            /// <summary>
            /// 自定义字段(文本)006 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD006")]
            public string Sfbaud006 { get; set; }
            /// <summary>
            /// 自定义字段(文本)007 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD007")]
            public string Sfbaud007 { get; set; }
            /// <summary>
            /// 自定义字段(文本)008 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD008")]
            public string Sfbaud008 { get; set; }
            /// <summary>
            /// 自定义字段(文本)009 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD009")]
            public string Sfbaud009 { get; set; }
            /// <summary>
            /// 品号对应代替料号 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD010")]
            public string Sfbaud010 { get; set; }
            /// <summary>
            /// 自定义字段(数字)011 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD011")]
            public decimal? Sfbaud011 { get; set; }
            /// <summary>
            /// 自定义字段(数字)012 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD012")]
            public decimal? Sfbaud012 { get; set; }
            /// <summary>
            /// 自定义字段(数字)013 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD013")]
            public decimal? Sfbaud013 { get; set; }
            /// <summary>
            /// 自定义字段(数字)014 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD014")]
            public decimal? Sfbaud014 { get; set; }
            /// <summary>
            /// 自定义字段(数字)015 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD015")]
            public decimal? Sfbaud015 { get; set; }
            /// <summary>
            /// 自定义字段(数字)016 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD016")]
            public decimal? Sfbaud016 { get; set; }
            /// <summary>
            /// 自定义字段(数字)017 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD017")]
            public decimal? Sfbaud017 { get; set; }
            /// <summary>
            /// 自定义字段(数字)018 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD018")]
            public decimal? Sfbaud018 { get; set; }
            /// <summary>
            /// 自定义字段(数字)019 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD019")]
            public decimal? Sfbaud019 { get; set; }
            /// <summary>
            /// 自定义字段(数字)020 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD020")]
            public decimal? Sfbaud020 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)021 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD021")]
            public DateTime? Sfbaud021 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)022 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD022")]
            public DateTime? Sfbaud022 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)023 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD023")]
            public DateTime? Sfbaud023 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)024 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD024")]
            public DateTime? Sfbaud024 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)025 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD025")]
            public DateTime? Sfbaud025 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)026 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD026")]
            public DateTime? Sfbaud026 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)027 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD027")]
            public DateTime? Sfbaud027 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)028 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD028")]
            public DateTime? Sfbaud028 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)029 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD029")]
            public DateTime? Sfbaud029 { get; set; }
            /// <summary>
            /// 自定义字段(日期时间)030 
            ///</summary>
            [SugarColumn(ColumnName = "SFBAUD030")]
            public DateTime? Sfbaud030 { get; set; }
            /// <summary>
            /// 备置量 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA031")]
            public decimal? Sfba031 { get; set; }
            /// <summary>
            /// 备置理由码 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA032")]
            public string Sfba032 { get; set; }
            /// <summary>
            /// 保税否 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA033")]
            public string Sfba033 { get; set; }
            /// <summary>
            /// SET被替代群组 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA034")]
            public string Sfba034 { get; set; }
            /// <summary>
            /// SET替代套数 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA035")]
            public decimal? Sfba035 { get; set; }
            /// <summary>
            /// SET已发料套数 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA036")]
            public decimal? Sfba036 { get; set; }
            /// <summary>
            /// 替代群组 
            ///</summary>
            [SugarColumn(ColumnName = "SFBA037")]
            public string Sfba037 { get; set; }
        }


        /// <summary>
        /// 物料名称档
        ///</summary>
        [SugarTable("IMAAL_T")]
        public class ImaalT
        {
            /// <summary>
            /// 企业编号 
            ///</summary>
            [SugarColumn(ColumnName = "IMAALENT", IsPrimaryKey = true)]
            public string Imaalent { get; set; }
            /// <summary>
            /// 料号 
            ///</summary>
            [SugarColumn(ColumnName = "IMAAL001", IsPrimaryKey = true)]
            public string Imaal001 { get; set; }
            /// <summary>
            /// 语种 
            ///</summary>
            [SugarColumn(ColumnName = "IMAAL002", IsPrimaryKey = true)]
            public string Imaal002 { get; set; }
            /// <summary>
            /// 名称 
            ///</summary>
            [SugarColumn(ColumnName = "IMAAL003")]
            public string Imaal003 { get; set; }
            /// <summary>
            /// 规格 
            ///</summary>
            [SugarColumn(ColumnName = "IMAAL004")]
            public string Imaal004 { get; set; }
            /// <summary>
            /// 助记码 
            ///</summary>
            [SugarColumn(ColumnName = "IMAAL005")]
            public string Imaal005 { get; set; }
        }
        /// <summary>
        /// 庫存明細檔
        ///</summary>
        [SugarTable("INAG_T")]
        public class InagT
        {
            /// <summary>
            /// 企业编号 
            ///</summary>
            [SugarColumn(ColumnName = "INAGENT", IsPrimaryKey = true)]
            public string Inagent { get; set; }
            /// <summary>
            /// 营运据点 
            ///</summary>
            [SugarColumn(ColumnName = "INAGSITE", IsPrimaryKey = true)]
            public string Inagsite { get; set; }
            /// <summary>
            /// 料件编号 
            ///</summary>
            [SugarColumn(ColumnName = "INAG001", IsPrimaryKey = true)]
            public string Inag001 { get; set; }
            /// <summary>
            /// 产品特征 
            ///</summary>
            [SugarColumn(ColumnName = "INAG002", IsPrimaryKey = true)]
            public string Inag002 { get; set; }
            /// <summary>
            /// 库存管理特征 
            ///</summary>
            [SugarColumn(ColumnName = "INAG003", IsPrimaryKey = true)]
            public string Inag003 { get; set; }
            /// <summary>
            /// 库位编号 
            ///</summary>
            [SugarColumn(ColumnName = "INAG004", IsPrimaryKey = true)]
            public string Inag004 { get; set; }
            /// <summary>
            /// 储位编号 
            ///</summary>
            [SugarColumn(ColumnName = "INAG005", IsPrimaryKey = true)]
            public string Inag005 { get; set; }
            /// <summary>
            /// 批号 
            ///</summary>
            [SugarColumn(ColumnName = "INAG006", IsPrimaryKey = true)]
            public string Inag006 { get; set; }
            /// <summary>
            /// 库存单位 
            ///</summary>
            [SugarColumn(ColumnName = "INAG007", IsPrimaryKey = true)]
            public string Inag007 { get; set; }
            /// <summary>
            /// 账面库存数量 
            ///</summary>
            [SugarColumn(ColumnName = "INAG008")]
            public decimal? Inag008 { get; set; }
            /// <summary>
            /// 实际库存数量 
            ///</summary>
            [SugarColumn(ColumnName = "INAG009")]
            public decimal? Inag009 { get; set; }
            /// <summary>
            /// 库存可用否 
            ///</summary>
            [SugarColumn(ColumnName = "INAG010")]
            public string Inag010 { get; set; }
            /// <summary>
            /// MRP可用否 
            ///</summary>
            [SugarColumn(ColumnName = "INAG011")]
            public string Inag011 { get; set; }
            /// <summary>
            /// 成本库否 
            ///</summary>
            [SugarColumn(ColumnName = "INAG012")]
            public string Inag012 { get; set; }
            /// <summary>
            /// 拣货优先序 
            ///</summary>
            [SugarColumn(ColumnName = "INAG013")]
            public string Inag013 { get; set; }
            /// <summary>
            /// 最近一次盘点日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG014")]
            public DateTime? Inag014 { get; set; }
            /// <summary>
            /// 最后异动日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG015")]
            public DateTime? Inag015 { get; set; }
            /// <summary>
            /// 呆滞日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG016")]
            public DateTime? Inag016 { get; set; }
            /// <summary>
            /// 第一次入库日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG017")]
            public DateTime? Inag017 { get; set; }
            /// <summary>
            /// No Use 
            ///</summary>
            [SugarColumn(ColumnName = "INAG018")]
            public DateTime? Inag018 { get; set; }
            /// <summary>
            /// 留置否 
            ///</summary>
            [SugarColumn(ColumnName = "INAG019")]
            public string Inag019 { get; set; }
            /// <summary>
            /// 留置原因 
            ///</summary>
            [SugarColumn(ColumnName = "INAG020")]
            public string Inag020 { get; set; }
            /// <summary>
            /// 备置数量 
            ///</summary>
            [SugarColumn(ColumnName = "INAG021")]
            public decimal? Inag021 { get; set; }
            /// <summary>
            /// No Use 
            ///</summary>
            [SugarColumn(ColumnName = "INAG022")]
            public string Inag022 { get; set; }
            /// <summary>
            /// Tag二进位码 
            ///</summary>
            [SugarColumn(ColumnName = "INAG023")]
            public string Inag023 { get; set; }
            /// <summary>
            /// 参考单位 
            ///</summary>
            [SugarColumn(ColumnName = "INAG024")]
            public string Inag024 { get; set; }
            /// <summary>
            /// 参考数量 
            ///</summary>
            [SugarColumn(ColumnName = "INAG025")]
            public decimal? Inag025 { get; set; }
            /// <summary>
            /// 最近一次检验日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG026")]
            public DateTime? Inag026 { get; set; }
            /// <summary>
            /// 下次检验日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG027")]
            public DateTime? Inag027 { get; set; }
            /// <summary>
            /// 留置日期 
            ///</summary>
            [SugarColumn(ColumnName = "INAG028")]
            public DateTime? Inag028 { get; set; }
            /// <summary>
            /// 留置人员 
            ///</summary>
            [SugarColumn(ColumnName = "INAG029")]
            public string Inag029 { get; set; }
            /// <summary>
            /// 留置部门 
            ///</summary>
            [SugarColumn(ColumnName = "INAG030")]
            public string Inag030 { get; set; }
            /// <summary>
            /// 留置单号 
            ///</summary>
            [SugarColumn(ColumnName = "INAG031")]
            public string Inag031 { get; set; }
            /// <summary>
            /// 基础单位 
            ///</summary>
            [SugarColumn(ColumnName = "INAG032")]
            public string Inag032 { get; set; }
            /// <summary>
            /// 基础单位数量 
            ///</summary>
            [SugarColumn(ColumnName = "INAG033")]
            public decimal? Inag033 { get; set; }
            /// <summary>
            /// 更新日期时间 
            ///</summary>
            [SugarColumn(ColumnName = "INAG034")]
            public DateTime? Inag034 { get; set; }
            /// <summary>
            /// 受虚拟异动影响否 
            ///</summary>
            [SugarColumn(ColumnName = "INAG035")]
            public string Inag035 { get; set; }
        }
    }
}