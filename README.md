
<p  align="center">
    <span>English</span> |  
    <a href="https://www.donet5.com/Home/Doc">中文</a>
</p>
 


## Description
 

SqlSugar ORM is a library providing Object/Relational Mapping (ORM) support to applications, libraries, and frameworks.

Using SqlSugar is very simple , And it's powerful.



- Support Support CodeFirst data migration.
- Support Join query 、  Union all 、 Subquery 
- Support Configure the query  
- Support DbFirst import entity class from database, or use Generation Tool.
- Support one-to-many and many-to-many navigation properties
- Support MySql、SqlServer、Sqlite、Oracle 、 postgresql 、达梦、人大金仓 、神通数据库


##  Join query  
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

 
 
