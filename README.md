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
