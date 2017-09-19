# SqlSugar 4.X  API

In addition to EF, the most powerful ORM

## Contactinfomation  
Email:610262374@qq.com 
QQ Group:225982985

## Nuget 
Install-Package sqlSugar  (MySql、SqlServer、Sqlite、Oracle)  
Install-Package sqlSugarCore  (MySql、SqlServer、sqlite、Oracle)

## Here's the history version
https://github.com/sunkaixuan/SqlSugar/tree/master

##  1. Query

### 1.1 Create Connection
If you have system table permissions, use SystemTableConfig,else use AttributeConfig
```c
SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
```

### 1.2 Introduction
```c
var getAll = db.Queryable<Student>().ToList();
var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
var getByFuns = db.Queryable<Student>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
var sum = db.Queryable<Student>().Sum(it=>it.Id);
var isAny = db.Queryable<Student>().Where(it=>it.Id==-1).Any();
var isAny2 = db.Queryable<Student>().Any(it => it.Id == -1);
var getListByRename = db.Queryable<School>().AS("Student").ToList();
var group = db.Queryable<Student>().GroupBy(it => it.Id)
.Having(it => SqlFunc.AggregateCount(it.Id) > 10)
.Select(it =>new { id = SqlFunc.AggregateCount(it.Id) }).ToList();
``` 

### 1.3 Page
```c
var pageIndex = 1;
var pageSize = 2;
var totalCount = 0;
//page
var page = db.Queryable<Student>().ToPageList(pageIndex, pageSize, ref totalCount);

//page join
var pageJoin = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
}).ToPageList(pageIndex, pageSize, ref totalCount);

//top 5
var top5 = db.Queryable<Student>().Take(5).ToList();

//skip5
var skip5 = db.Queryable<Student>().Skip(5).ToList();
``` 

### 1.4 Join

#### easy join 
```c
//2 join
var list5 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select((st,sc)=>new {st.Name,st.Id,schoolName=sc.Name}).ToList();
```

```c
//3 join 
var list6 = db.Queryable<Student, School,School>((st, sc,sc2) => st.SchoolId == sc.Id&&sc.Id==sc2.Id)
    .Select((st, sc,sc2) => new { st.Name, st.Id, schoolName = sc.Name,schoolName2=sc2.Name }).ToList();
 ```
 
 ```c
//3 join page
var list7= db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
.Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToPageList(1,2);
```

#### left join  
```c
//join  2
var list = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
})
.Where(st=>st.Name=="jack").ToList();

//join  3
var list2 = db.Queryable<Student, School,Student>((st, sc,st2) => new object[] {
JoinType.Left,st.SchoolId==sc.Id,
JoinType.Left,st.SchoolId==st2.Id
})
.Where((st, sc, st2)=> st2.Id==1||sc.Id==1||st.Id==1).ToList();

//join return List<ViewModelStudent>
var list3 = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
}).Select((st,sc)=>new ViewModelStudent { Name= st.Name,SchoolId=sc.Id }).ToList();

//join Order By (order by st.id desc,sc.id desc)
var list4 = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
})
.OrderBy(st=>st.Id,OrderByType.Desc)
.OrderBy((st,sc)=>sc.Id,OrderByType.Desc)
.Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();
```

### subquery
```c
var getAll = db.Queryable<Student, School>((st, sc) => new object[] {
 JoinType.Left,st.Id==sc.Id})
.Where(st => st.Id == SqlFunc.Subqueryable<School>().Where(s => s.Id == st.Id).Select(s => s.Id))
.ToList();
      
//sql
SELECT `st`.`ID`,`st`.`SchoolId`,`st`.`Name`,`st`.`CreateTime` 
     FROM `STudent` st Left JOIN `School` sc ON ( `st`.`ID` = `sc`.`Id` )  
      WHERE ( `st`.`ID` =(SELECT `Id` FROM `School` WHERE ( `Id` = `st`.`ID` ) limit 0,1))
```

### 1.5 SqlFunctions
```c
var t1 = db.Queryable<Student>().Where(it => SqlFunc.ToLower(it.Name) == SqlFunc.ToLower("JACK")).ToList();
//SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  WHERE ((LOWER([Name])) = (LOWER(@MethodConst0)) )

/***More Functions***/
//SqlFunc.IsNullOrEmpty(object thisValue)
//SqlFunc.ToLower(object thisValue) 
//SqlFunc.string ToUpper(object thisValue) 
//SqlFunc.string Trim(object thisValue) 
//SqlFunc.bool Contains(string thisValue, string parameterValue) 
//SqlFunc.ContainsArray(object[] thisValue, string parameterValue) 
//SqlFunc.StartsWith(object thisValue, string parameterValue) 
//SqlFunc.EndsWith(object thisValue, string parameterValue)
//SqlFunc.Equals(object thisValue, object parameterValue) 
//SqlFunc.DateIsSame(DateTime date1, DateTime date2)
//SqlFunc.DateIsSame(DateTime date1, DateTime date2, DateType dataType) 
//SqlFunc.DateAdd(DateTime date, int addValue, DateType millisecond) 
//SqlFunc.DateAdd(DateTime date, int addValue) 
//SqlFunc.DateValue(DateTime date, DateType dataType) 
//SqlFunc.Between(object value, object start, object end) 
//SqlFunc.ToInt32(object value) 
//SqlFunc.ToInt64(object value)
//SqlFunc.ToDate(object value) 
//SqlFunc.ToString(object value) 
//SqlFunc.ToDecimal(object value) 
//SqlFunc.ToGuid(object value) 
//SqlFunc.ToDouble(object value) 
//SqlFunc.ToBool(object value) 
//SqlFunc.Substring(object value, int index, int length)
//SqlFunc.Replace(object value, string oldChar, string newChar)
//SqlFunc.Length(object value) { throw new NotImplementedException(); }
//SqlFunc.AggregateSum(object thisValue) 
//SqlFunc.AggregateAvg<TResult>(TResult thisValue)
//SqlFunc.AggregateMin(object thisValue) 
//SqlFunc.AggregateMax(object thisValue) 
//SqlFunc.AggregateCount(object thisValue) 
```

### 1.6 Select
```c
var s1 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Name = it.Name, Student = it }).ToList();
var s2 = db.Queryable<Student>().Select(it => new { id = it.Id, w = new { x = it } }).ToList();
var s3 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
var s4 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
var s5 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it, Name = it.Name }).ToList();
```

### 1.7  Join Sql
```c
var join3 = db.Queryable("Student", "st")
                .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                .Where("st.id>@id")
                .AddParameters(new { id = 1 })
                .Select("st.*").ToList();
 //SELECT st.* FROM [Student] st Left JOIN School sh ON sh.id=st.schoolid   WHERE st.id>@id 
 ```
 
 ### 1.8 ADO.NET
 ```c
var db = GetInstance();
var t1= db.Ado.SqlQuery<string>("select 'a'");
var t2 = db.Ado.GetInt("select 1");
var t3 = db.Ado.GetDataTable("select 1 as id");
//more
//db.Ado.GetXXX...
 ```

### 1.9 Where
```c
var list = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
})
.Where((st,sc)=> sc.Id == 1)
.Where((st,sc) => st.Id == 1)
.Where((st, sc) => st.Id == 1 && sc.Id == 2).ToList();

//SELECT [st].[Id],[st].[SchoolId],[st].[Name],[st].[CreateTime] FROM [Student] st 
//Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   
//WHERE ( [sc].[Id] = @Id0 )  AND ( [st].[Id] = @Id1 )  AND (( [st].[Id] = @Id2 ) AND ( [sc].[Id] = @Id3 ))

//Where If
string name = null;
string name2 = "sunkaixuan";
var list2 = db.Queryable<Student>()
.WhereIF(!string.IsNullOrEmpty(name), it => it.Name == name)
.WhereIF(!string.IsNullOrEmpty(name2), it => it.Name == name2).ToList();
```

 
##  2. Insert
 ```c
var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1") ,SchoolId=1};

//Insert reutrn Insert Count
var t2 = db.Insertable(insertObj).ExecuteCommand();

//Insert reutrn Identity Value
var t3 = db.Insertable(insertObj).ExecuteReutrnIdentity();


//Only  insert  Name 
var t4 = db.Insertable(insertObj).InsertColumns(it => new { it.Name,it.SchoolId }).ExecuteReutrnIdentity();


//Ignore TestId
var t5 = db.Insertable(insertObj).IgnoreColumns(it => new { it.Name, it.TestId }).ExecuteReutrnIdentity();


//Ignore   TestId
var t6 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").ExecuteReutrnIdentity();


//Use Lock
var t8 = db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();


var insertObj2 = new Student() { Name = null, CreateTime = Convert.ToDateTime("2010-1-1") };
var t9 = db.Insertable(insertObj2).Where(true/* Is insert null */, true/*off identity*/).ExecuteCommand();

//Insert List<T>
var insertObjs = new List<Student>();
for (int i = 0; i < 1000; i++)
{
 insertObjs.Add(new Student() { Name = "name" + i });
}
var s9 = db.Insertable(insertObjs.ToArray()).InsertColumns(it => new { it.Name }).ExecuteCommand();
```
##  3. Delete

 ```c
var t1 = db.Deleteable<Student>().Where(new Student() { Id = 1 }).ExecuteCommand();

//use lock
var t2 = db.Deleteable<Student>().With(SqlWith.RowLock).ExecuteCommand();


//by primary key
var t3 = db.Deleteable<Student>().In(1).ExecuteCommand();

//by primary key array
var t4 = db.Deleteable<Student>().In(new int[] { 1, 2 }).ExecuteCommand();

//by expression
var t5 = db.Deleteable<Student>().Where(it => it.Id == 1).ExecuteCommand();
 ```
 ##  4. Update

 ```c
//update reutrn Update Count
var t1= db.Updateable(updateObj).ExecuteCommand();

//Only  update  Name 
var t3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name }).ExecuteCommand();


//Ignore  Name and TestId
var t4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ExecuteCommand();

//Ignore  Name and TestId
var t5 = db.Updateable(updateObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ExecuteCommand();


//Use Lock
var t6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ExecuteCommand();

//update List<T>
var t7 = db.Updateable(updateObjs).ExecuteCommand();

//Re Set Value
var t8 = db.Updateable(updateObj)
 .ReSetValue(it => it.Name == (it.Name + 1)).ExecuteCommand();

//Where By Expression
var t9 = db.Updateable(updateObj).Where(it => it.Id == 1).ExecuteCommand();

//Update By Expression  Where By Expression
var t10 = db.Updateable<Student>()
 .UpdateColumns(it => new Student() { Name="a",CreateTime=DateTime.Now })
 .Where(it => it.Id == 11).ExecuteCommand();
 ```

 ##  5. Table structure is different from entity
 ##### Priority level： 
 AS>Add>Attribute
 ### 5.1 Add
 ```c
db.MappingTables.Add()
db.MappingColumns.Add()
db.IgnoreColumns.Add()
 ```
 ### 5.2 AS
  ```c
db.Queryable<T>().As("tableName").ToList();
 ```
### 5.3 Attribute
 ```c
[SugarColumn(IsIgnore=true)]
public int TestId { get; set; }
  ```

 ##  6. Use Tran
  ```c
var result = db.Ado.UseTran(() =>
{
          
    var beginCount = db.Queryable<Student>().ToList();
    db.Ado.ExecuteCommand("delete student");
    var endCount = db.Queryable<Student>().Count();
    throw new Exception("error haha");
})
   ```
 ##  7. Use Store Procedure
```c
 
var dt2 = db.Ado.UseStoredProcedure().GetDataTable("sp_school",new{p1=1,p2=null});
 
//output
var p11 = new SugarParameter("@p1", "1");
var p22 = new SugarParameter("@p2", null, true);//isOutput=true
var dt2 = db.Ado.UseStoredProcedure().GetDataTable("sp_school",p11,p22);
```

## 8. DbFirst
```c
var db = GetInstance();
    //Create all class
    db.DbFirst.CreateClassFile("c:\\Demo\\1");

    //Create student calsss
    db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\2");
    //Where(array)

    //Mapping name
    db.MappingTables.Add("ClassStudent", "Student");
    db.MappingColumns.Add("NewId", "Id", "ClassStudent");
    db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\3");

    //Remove mapping
    db.MappingTables.Clear();

    //Create class with default value
    db.DbFirst.IsCreateDefaultValue().CreateClassFile("c:\\Demo\\4", "Demo.Models");


    //Mapping and Attribute
    db.MappingTables.Add("ClassStudent", "Student");
    db.MappingColumns.Add("NewId", "Id", "ClassStudent");
    db.DbFirst.IsCreateAttribute().Where("Student").CreateClassFile("c:\\Demo\\5");


    //Remove mapping
    db.MappingTables.Clear();
    db.MappingColumns.Clear();

    //Custom format,Change old to new
    db.DbFirst.
        SettingClassTemplate(old =>
        {
            return old;
        })
       .SettingNamespaceTemplate(old =>
       {
           return old;
       })
       .SettingPropertyDescriptionTemplate(old =>
       {
           return @"           /// <summary>
   /// Desc_New:{PropertyDescription}
   /// Default_New:{DefaultValue}
   /// Nullable_New:{IsNullable}
   /// </summary>";
       })
        .SettingPropertyTemplate(old =>
        {
            return old;
        })
        .SettingConstructorTemplate(old =>
        {
            return old;
        })
    .CreateClassFile("c:\\Demo\\6");
}
```
## 8.Code First
```
db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2)); //change entity backupTable
db.CodeFirst.InitTables(typeof(CodeTable),typeof(CodeTable2));
```

## 9. AOP LOG
```
db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
{
 
};
db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
{
 
};
db.Aop.OnError = (exp) =>//执行SQL 错误事件
{
                 
};
db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL执行前 可以修改SQL
{
    return new KeyValuePair<string, SugarParameter[]>(sql,pars);
};

```

More
http://www.codeisbug.com/Doc/8
