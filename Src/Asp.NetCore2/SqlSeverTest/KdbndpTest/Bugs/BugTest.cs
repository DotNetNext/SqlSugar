//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrmTest.Test
//{
//    public class BugTest
//    {
//        public static void Init()
//        {
//            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
//            {
//                ConnectionString = @"PORT=5433;DATABASE=x;HOST=localhost;PASSWORD=haosql;USER ID=postgres",
//                DbType = DbType.Kdbndp,
//                IsAutoCloseConnection = true,
//                //MoreSettings = new ConnMoreSettings()
//                //{
//                //    PgSqlIsAutoToLower = true //我们这里需要设置为false
//                //},
//                InitKeyType = InitKeyType.Attribute,
//            });
//            //调式代码 用来打印SQL 
//            Db.Aop.OnLogExecuting = (sql, pars) =>
//            {
//                // Debug.WriteLine(sql);
//            };

//            var 查询成功 = Db.Queryable<Sys_Menu>()
//                .Where(x => x.Enable == true) //不加 ture ，也会出错
//                .ToList();


//            var Dbfirst情况使用Mapper_Menu_Id为大写自动转成小写 = Db.Queryable<Sys_Menu>()
//                .Where(x => x.Enable == true)
//                .Mapper(x => x.orderList, x => x.Menu_Id) //初始化数据库失败42703: 字段 "menu_id" 不存在
//                .ToList();
//        }
//    }
//    public class Sys_RoleAuth
//    {
//        /// <summary>
//        ///
//        /// </summary>
//        [Key]
//        [Display(Name = "")]
//        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
//        public int Auth_Id { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        public int Role_Id { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        public int User_Id { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        [SugarColumn(IsNullable = false)]
//        public int Menu_Id { get; set; }

//        /// <summary>
//        ///用户权限
//        /// </summary>
//        [Display(Name = "用户权限")]
//        [SugarColumn(IsNullable = false, IsJson = true, ColumnDataType = "json")]
//        public List<Sys_Actions> AuthValue { get; set; } = new List<Sys_Actions>();

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        [MaxLength(100)]
//        public string Creator { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        [MaxLength(100)]
//        public string Modifier { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "")]
//        public DateTime ModifyDate { get; set; }
//    }
//    public class Sys_Actions
//    {
//        public int Action_Id { get; set; }
//        public int Menu_Id { get; set; }
//        public string Text { get; set; }
//        public string Value { get; set; }
//    }
//    public class Sys_Menu
//    {
//        /// <summary>
//        ///ID
//        /// </summary>
//        [Key]
//        [Display(Name = "ID")]
//        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
//        public int Menu_Id { get; set; }

//        /// <summary>
//        ///父级ID
//        /// </summary>
//        [Display(Name = "父级ID")]
//        [SugarColumn(IsNullable = false)]
//        public int ParentId { get; set; }


//        /// <summary>
//        ///菜单名称
//        /// </summary>
//        [Display(Name = "菜单名称")]
//        [MaxLength(50)]
//        [SugarColumn(IsNullable = false)]
//        public string MenuName { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "TableName")]
//        [MaxLength(200)]
//        public string TableName { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "Url")]
//        [MaxLength(10000)]
//        public string Url { get; set; }

//        /// <summary>
//        ///权限
//        /// </summary>
//        [Display(Name = "权限")]
//        [MaxLength(-1)]
//        [SugarColumn(IsNullable = false, IsJson = true, ColumnDataType = "json")]
//        public List<Sys_Actions> Auth { get; set; } = new List<Sys_Actions>();

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "Description")]
//        [MaxLength(200)]
//        public string Description { get; set; }


//        /// <summary>
//        ///图标
//        /// </summary>
//        [Display(Name = "图标")]
//        [MaxLength(50)]
//        public string Icon { get; set; }

//        /// <summary>
//        ///排序号
//        /// </summary>
//        [Display(Name = "排序号")]
//        public int OrderNo { get; set; }

//        /// <summary>
//        ///创建人
//        /// </summary>
//        [Display(Name = "创建人")]
//        [MaxLength(50)]
//        public string Creator { get; set; }

//        /// <summary>
//        ///创建时间
//        /// </summary>
//        [Display(Name = "创建时间")]
//        public DateTime? CreateDate { get; set; } = DateTime.Now;

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "Modifier")]
//        [MaxLength(50)]
//        public string Modifier { get; set; }

//        /// <summary>
//        ///
//        /// </summary>
//        [Display(Name = "ModifyDate")]
//        public DateTime? ModifyDate { get; set; }

//        /// <summary>
//        ///是否启用
//        /// </summary>
//        [Display(Name = "是否启用")]
//        public bool Enable { get; set; }

//        [SugarColumn(IsIgnore = true)]
//        public List<Order> orderList { get; set; }
//    }

//    public class Order
//    {
//        [Key]
//        [Display(Name = "ID")]
//        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
//        public int Id { get; set; }

//        public int Menu_Id { get; set; }
//    }

//}
