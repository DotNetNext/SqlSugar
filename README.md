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
}).ToList();

//join  3
var list2 = db.Queryable<Student, School,Student>((st, sc,st2) => new object[] {
JoinType.Left,st.SchoolId==sc.Id,
JoinType.Left,st.SchoolId==st2.Id
}).ToList();

//join return List<ViewModelStudent>
var list3 = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
}).Select<Student,School,ViewModelStudent>((st,sc)=>new ViewModelStudent { Name= st.Name,SchoolId=sc.Id }).ToList();

//join Order By (order by st.id desc,sc.id desc)
var list4 = db.Queryable<Student, School>((st, sc) => new object[] {
JoinType.Left,st.SchoolId==sc.Id
})
.OrderBy(st=>st.Id,OrderByType.Desc)
.OrderBy<School>(sc=>sc.Id,OrderByType.Desc)
.Select<Student, School, ViewModelStudent>((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();
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
 
