using SqlSugar;
using System.Collections.Generic;

namespace OrmTest
{
    internal class _a1_Delete
    {
        /// <summary>
        /// 初始化删除操作的示例方法。
        /// Initializes example methods for delete operations.
        /// </summary>
        internal static void Init()
        {
            // 获取新的数据库连接对象
            // Get a new database connection object
            var db = DbHelper.GetNewDb();

            db.CodeFirst.InitTables<Student, Order>();

            // 调用各个删除操作的示例方法
            // Calling example methods for various delete operations
            DeleteSingleEntity(db);

            // 批量删除实体的示例方法
            // Example method for deleting entities in batch
            DeleteBatchEntities(db);

            // 批量删除并分页的示例方法
            // Example method for deleting entities in batch with paging
            DeleteBatchEntitiesWithPaging(db);
             
            // 调用无主键实体删除的示例方法
            // Calling example method for deleting entities without primary key
            DeleteEntitiesWithoutPrimaryKey(db);

            // 调用根据主键删除实体的示例方法
            // Calling example method for deleting entity by primary key
            DeleteEntityByPrimaryKey(1, db);
             
            // 调用根据主键数组批量删除实体的示例方法
            // Calling example method for deleting entities by primary key array
            DeleteEntitiesByPrimaryKeyArray(db);

            // 调用根据表达式删除实体的示例方法
            // Calling example method for deleting entities by expression
            DeleteEntitiesByExpression(db);

            // 调用联表删除实体的示例方法
            // Calling example method for deleting entities with join
            DeleteEntitiesWithJoin(db);
        }

        /// <summary>
        /// 删除单个实体的示例方法。
        /// Example method for deleting a single entity.
        /// </summary>
        internal static void DeleteSingleEntity(ISqlSugarClient db)
        {
            // 删除指定 Id 的学生实体
            // Delete the student entity with the specified Id
            db.Deleteable<Student>(new Student() { Id = 1 }).ExecuteCommand();
        }

        /// <summary>
        /// 批量删除实体的示例方法。
        /// Example method for deleting entities in batch.
        /// </summary>
        internal static void DeleteBatchEntities(ISqlSugarClient db)
        {
            // 创建学生实体列表
            // Create a list of student entities
            List<Student> list = new List<Student>()
            {
                new Student() { Id = 1 }
            };

            // 批量删除学生实体
            // Delete student entities in batch
            db.Deleteable<Student>(list).ExecuteCommand();
        }

        /// <summary>
        /// 批量删除并分页的示例方法。
        /// Example method for deleting entities in batch with paging.
        /// </summary>
        internal static void DeleteBatchEntitiesWithPaging(ISqlSugarClient db)
        {
            // 创建订单实体列表
            // Create a list of order entities
            List<Order> list = new List<Order>();

            // 批量删除订单实体并分页
            // Delete order entities in batch with paging
            db.Deleteable<Order>(list).PageSize(500).ExecuteCommand();
        }

        /// <summary>
        /// 无主键实体删除的示例方法。
        /// Example method for deleting entities without primary key.
        /// </summary>
        internal static void DeleteEntitiesWithoutPrimaryKey( ISqlSugarClient db)
        {
            List<Order> orders = new List<Order>()
            {
                new Order() { Id = 1 },
                new Order() { Id = 2 }
            };

            // 根据指定的实体列表的 Id 列进行删除
            // Delete entities based on the Id column of the specified entity list
            db.Deleteable<Order>().WhereColumns(orders, it => new { it.Id }).ExecuteCommand();
        }

        /// <summary>
        /// 根据主键删除实体的示例方法。
        /// Example method for deleting an entity by primary key.
        /// </summary>
        internal static void DeleteEntityByPrimaryKey(int id, ISqlSugarClient db)
        {
            // 根据指定的 Id 删除学生实体
            // Delete the student entity with the specified Id
            db.Deleteable<Student>().In(id).ExecuteCommand();
        }

        /// <summary>
        /// 根据主键数组批量删除实体的示例方法。
        /// Example method for deleting entities by primary key array.
        /// </summary>
        internal static void DeleteEntitiesByPrimaryKeyArray(ISqlSugarClient db)
        {
            // 定义主键数组
            // Define an array of primary keys
            int[] ids = { 1, 2 };

            // 根据指定的 Id 数组批量删除学生实体
            // Delete student entities in batch based on the specified Id array
            db.Deleteable<Student>().In(ids).ExecuteCommand();
        }

        /// <summary>
        /// 根据表达式删除实体的示例方法。
        /// Example method for deleting entities by expression.
        /// </summary>
        internal static void DeleteEntitiesByExpression(ISqlSugarClient db)
        {
            // 根据指定的表达式删除学生实体
            // Delete the student entity based on the specified expression
            db.Deleteable<Student>().Where(it => it.Id == 1).ExecuteCommand();
        }

        /// <summary>
        /// 联表删除实体的示例方法。
        /// Example method for deleting entities with join.
        /// </summary>
        internal static void DeleteEntitiesWithJoin(ISqlSugarClient db)
        {
            // 联表删除学生实体
            // Delete student entities with join
            db.Deleteable<Student>()
              .Where(p => SqlFunc.Subqueryable<Order>().Where(s => s.Id == p.SchoolId).Any())
              .ExecuteCommand();
        }

        [SugarTable("Students_a1")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] // 主键
            public int Id { get; set; }

            public string Name { get; set; }

            public int SchoolId { get; set; }
        }

        [SugarTable("Orders_a2")]
        public class Order
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] // 主键
            public int Id { get; set; }

            public string OrderNumber { get; set; }

            public decimal Amount { get; set; }
        }
    }
}