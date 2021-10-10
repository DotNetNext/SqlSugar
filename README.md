
<p  align="center">
    <span>English</span> |  
    <a href="https://www.donet5.com/Home/Doc">中文</a>
</p>
 


## 🍬 SqlSugar 

SqlSugar ORM is a library providing Object/Relational Mapping (ORM) support to applications, libraries, and frameworks.

Using SqlSugar is very simple , And it's powerful.


- Multi-tenant, multi-library transactions
- Support Support CodeFirst data migration.
- Support Join query 、  Union all 、 Subquery 
- Support Configure the query  
- Support DbFirst import entity class from database, or use Generation Tool.
- Support one-to-many and many-to-many navigation properties
- Support MySql、SqlServer、Sqlite、Oracle 、 postgresql 、达梦、人大金仓 、神通数据库


##  🍳 Join query  
Super simple query syntax
```cs
var query5 = db.Queryable<Order>()
            .LeftJoin<Custom>  ((o, cus) => o.CustomId == cus.Id)
            .LeftJoin<OrderItem> ((o, cus, oritem ) => o.Id == oritem.OrderId)
            .LeftJoin<OrderItem> ((o, cus, oritem , oritem2) => o.Id == oritem2.OrderId)
            .Where(o => o.Id == 1)  
            .Select((o, cus) => new ViewOrder { Id = o.Id, CustomName = cus.Name })
            .ToList();   
```
```sql
SELECT
  [o].[Id] AS [Id],
  [cus].[Name] AS [CustomName]
FROM
  [Order] o
  Left JOIN [Custom] cus ON ([o].[CustomId] = [cus].[Id])
  Left JOIN [OrderDetail] oritem ON ([o].[Id] = [oritem].[OrderId])
  Left JOIN [OrderDetail] oritem2 ON ([o].[Id] = [oritem2].[OrderId])
WHERE
  ([o].[Id] = @Id0)
``` 

##  📑 Page query
```cs

 int pageIndex = 1; 
 int pageSize = 20;
 int totalCount=0;
 var page = db.Queryable<Student>().ToPageList(pageIndex, pageSize, ref totalCount);
```
 
##   🚗 Dynamic expression
```cs
var names= new string [] { "a","b"};
Expressionable<Order> exp = new Expressionable<Order>();
foreach (var item in names)
{
    exp.Or(it => it.Name.Contains(item.ToString()));
}
var list= db.Queryable<Order>().Where(exp.ToExpression()).ToList();
 ```
 ```sql
SELECT [Id],[Name],[Price],[CreateTime],[CustomId]
        FROM [Order]  WHERE (
                      ([Name] like '%'+ CAST(@MethodConst0 AS NVARCHAR(MAX))+'%') OR 
                      ([Name] like '%'+ CAST(@MethodConst1 AS NVARCHAR(MAX))+'%')
                     )
```
##  Multi-tenant transaction
```cs
//Creaate  database object
SqlSugarClient db = new SqlSugarClient(new List<ConnectionConfig>()
{
    new ConnectionConfig(){ ConfigId="0", DbType=DbType.SqlServer,  ConnectionString=Config.ConnectionString, IsAutoCloseConnection=true },
    new ConnectionConfig(){ ConfigId="1", DbType=DbType.MySql, ConnectionString=Config.ConnectionString4 ,IsAutoCloseConnection=true}
});


var mysqldb = db.GetConnection("1");//mysql db
var sqlServerdb = db.GetConnection("0");// sqlserver db
 
db.BeginTran();
            mysqldb.Insertable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();
            mysqldb.Queryable<Order>().ToList();
            sqlServerdb.Queryable<Order>().ToList();

db.CommitTran();
```
