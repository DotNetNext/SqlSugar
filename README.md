# SqlSugar 4.X  API
## Contactinfomation  
Email:610262374@qq.com 
QQ Group:225982985

## 3.x API
3.X https://github.com/sunkaixuan/SqlSugar/tree/master

##  1. Query

### 1.1 Create Connection
If you have system table permissions, use SystemTableConfig,else use AttributeConfig
```c
     SqlSugarClient db = new SqlSugarClient(new SystemTableConfig() 
     { ConnectionString = Config.ConnectionString, DbType =DbType.SqlServer, IsAutoCloseConnection = true });
```

SystemTableConfig：
https://github.com/sunkaixuan/SqlSugar/wiki/SystemTableConfig

AttrbuteConfig
https://github.com/sunkaixuan/SqlSugar/wiki/AttributeCofnig


### 1.2 Introduction
```c
var getAll = db.Queryable<Student>().ToList();
var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
var getByFuns = db.Queryable<Student>().Where(it => NBORM.IsNullOrEmpty(it.Name)).ToList();
var sum = db.Queryable<Student>().Sum(it=>it.Id);
var isAny = db.Queryable<Student>().Where(it=>it.Id==-1).Any();
var isAny2 = db.Queryable<Student>().Any(it => it.Id == -1);
var getListByRename = db.Queryable<School>().AS("Student").ToList();
var group = db.Queryable<Student>().GroupBy(it => it.Id)
.Having(it => NBORM.AggregateCount(it.Id) > 10)
.Select(it =>new { id = NBORM.AggregateCount(it.Id) }).ToList();
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

### 1.5 SqlFunctions
```c
var t1 = db.Queryable<Student>().Where(it => NBORM.ToLower(it.Name) == NBORM.ToLower("JACK")).ToList();
//SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  WHERE ((LOWER([Name])) = (LOWER(@MethodConst0)) )

/***More Functions***/
//NBORM.IsNullOrEmpty(object thisValue)
//NBORM.ToLower(object thisValue) 
//NBORM.string ToUpper(object thisValue) 
//NBORM.string Trim(object thisValue) 
//NBORM.bool Contains(string thisValue, string parameterValue) 
//NBORM.ContainsArray(object[] thisValue, string parameterValue) 
//NBORM.StartsWith(object thisValue, string parameterValue) 
//NBORM.EndsWith(object thisValue, string parameterValue)
//NBORM.Equals(object thisValue, object parameterValue) 
//NBORM.DateIsSame(DateTime date1, DateTime date2)
//NBORM.DateIsSame(DateTime date1, DateTime date2, DateType dataType) 
//NBORM.DateAdd(DateTime date, int addValue, DateType millisecond) 
//NBORM.DateAdd(DateTime date, int addValue) 
//NBORM.DateValue(DateTime date, DateType dataType) 
//NBORM.Between(object value, object start, object end) 
//NBORM.ToInt32(object value) 
//NBORM.ToInt64(object value)
//NBORM.ToDate(object value) 
//NBORM.ToString(object value) 
//NBORM.ToDecimal(object value) 
//NBORM.ToGuid(object value) 
//NBORM.ToDouble(object value) 
//NBORM.ToBool(object value) 
//NBORM.Substring(object value, int index, int length)
//NBORM.Replace(object value, string oldChar, string newChar)
//NBORM.Length(object value) { throw new NotImplementedException(); }
//NBORM.AggregateSum(object thisValue) 
//NBORM.AggregateAvg<TResult>(TResult thisValue)
//NBORM.AggregateMin(object thisValue) 
//NBORM.AggregateMax(object thisValue) 
//NBORM.AggregateCount(object thisValue) 
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

 ##  4. Mapping
Priority level  AS>Add>Attribute
 ### 4.1 Add
 ```c
db.MappingTables.Add()
db.MappingColumns.Add()
db.IgnoreColumns.Add()
 ```
 ### 4.2 AS
  ```c
db.Queryable<T>().As("tableName").ToList();
 ```
### 4.3 Attribute
 ```c
[SugarColumn(IsIgnore=true)]
public int TestId { get; set; }
  ```

