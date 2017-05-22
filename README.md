# SqlSugar 4.X

##  1. Query

### 1.1 Create Connection
```c
     SqlSugarClient db = new SqlSugarClient(new SystemTableConfig() 
     { ConnectionString = Config.ConnectionString, DbType =DbType.SqlServer, IsAutoCloseConnection = true });
```

### 1.2 Introduction


