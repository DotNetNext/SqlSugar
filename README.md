# SqlSugar 4.X

##  1. Query

### 1.1 Create Connection
```c
     SqlSugarClient db = new SqlSugarClient(new SystemTableConfig() 
     { ConnectionString = Config.ConnectionString, DbType =DbType.SqlServer, IsAutoCloseConnection = true });
```

### 1.2 Introduction
```c
  var getAll = db.Queryable<Student>().ToList();
  var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
  var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
  var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
  var getByFuns = db.Queryable<Student>().Where(it => NBORM.IsNullOrEmpty(it.Name)).ToList();
``` 

