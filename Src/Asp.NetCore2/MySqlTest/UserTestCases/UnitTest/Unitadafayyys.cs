using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadafayyys
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            if (!db.DbMaintenance.IsAnyTable("wms137m"))
                db.Ado.ExecuteCommand(
                    "CREATE TABLE `wms137m` (\r\n\r\n  `id` int NOT NULL AUTO_INCREMENT,\r\n\r\n  `moveout_doc` varchar(1000) NOT NULL DEFAULT '' COMMENT '出库单号',\r\n\r\n  `moveout_date` varchar(1000) NOT NULL DEFAULT '' COMMENT '出库日期',\r\n\r\n  `suppliers_code` varchar(1000) NOT NULL DEFAULT '' COMMENT '供应商',\r\n\r\n  `customer_code` varchar(1000) NOT NULL DEFAULT '' COMMENT '客户',\r\n\r\n`front_document_type` int NOT NULL  DEFAULT 4001 COMMENT '前置单据类型 4001:调拨拨出 4002:调拨拨入',\r\n\r\n  `front_document` varchar(50) NOT NULL DEFAULT '' COMMENT '前置单据',\r\n\r\n  `Remarks` varchar(1000) NOT NULL DEFAULT '' COMMENT '备注',\r\n\r\n  `status` int NOT NULL DEFAULT 8 COMMENT '状态',\r\n\r\n  `createby` varchar(100) NOT NULL DEFAULT '' COMMENT '创建人',\r\n\r\n  `createdate` varchar(100) NOT NULL DEFAULT '' COMMENT '创建日期',\r\n\r\n  `createtime` varchar(100) NOT NULL DEFAULT '' COMMENT '创建时间',\r\n\r\n  `modifyby` varchar(100) NOT NULL DEFAULT '' COMMENT '修改人',\r\n\r\n  `modifydate` varchar(100) NOT NULL DEFAULT '' COMMENT '修改日期',\r\n\r\n  `modifytime` varchar(100) NOT NULL DEFAULT '' COMMENT '修改时间',\r\n\r\n  `pick_person` varchar(50) NOT NULL DEFAULT '' COMMENT '领料人',\r\n\r\n  `erp_key` varchar(255) NOT NULL DEFAULT '',\r\n\r\n  PRIMARY KEY (`id`)\r\n\r\n) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC COMMENT='调拨出库单';");

            foreach (var item in db.DbMaintenance.GetColumnInfosByTableName("wms137m",false))
            {
                Console.WriteLine($"{item.DbColumnName} default {item.DefaultValue}");
            }

            db.DbFirst.IsCreateDefaultValue().Where(it => it == "wms137m")
                .CreateClassFile("c:\\demo\\1\\01");
        }
    }
}
