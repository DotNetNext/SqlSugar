
<iframe width="400" height="500" src="http://www.codeisbug.com">
  </iframe>
  
#Contact information

Email 610262374@qq.com

QQ Group 225982985

Blog http://www.cnblogs.com/sunkaixuan


#All versions

ASP.NET 4.0+ (MSSQL , MYSQL ORACLE ,SQLITE Four in one) https://github.com/sunkaixuan/SqlSugarRepository

ASP.NET 4.0+ MSSQL   https://github.com/sunkaixuan/SqlSugar

ASP.NET CORE MSSQL   https://github.com/sunkaixuan/ASP_NET_CORE_ORM_SqlSugar

ASP.NET 4.0+ MYSQL   https://github.com/sunkaixuan/MySqlSugar

ASP.NET CORE MYSQL   https://github.com/sunkaixuan/ASP_NET_CORE_ORM_MySqlSugar

ASP.NET 4.0+ Sqlite  https://github.com/sunkaixuan/SqliteSugar

ASP.NET CORE Sqlite  https://github.com/sunkaixuan/ASP_NET_CORE_ORM_SqliteSugar

ASP.NET 4.0+ ORACLE  https://github.com/sunkaixuan/OracleSugar

ASP.NET CORE ORACLE  https://github.com/sunkaixuan/ASP_NET_CORE_ORM_OracleSugar


# Instance SqlSugar object

```csharp
using(var db = new SqlSugarClient(ConnectionString)){

	//use object
	var list=db.Queryable<T>().ToList();
	
}

```

# Package instance 
```csharp
/// <summary>
/// SqlSugar
/// </summary>
public class SugarDao
{
	private SugarDao()
	{

	}
	public static string ConnectionString
	{
		get
		{
			string reval = "server=.;uid=sa;pwd=sasa;database=SqlSugarTest"; 
			return reval;
		}
	}
	public static SqlSugarClient GetInstance()
	{
		var db = new SqlSugarClient(ConnectionString);
		db.IsEnableLogEvent = true;//Enable log events
		db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par+"\r\n"); };
		return db;
	}
}

```
##### Use SugarDao
```csharp
using (var db = SugarDao.GetInstance())
{
	var list=db.Queryable<T>().ToList();
}
```



# 1.Select 

##### 1.1 queryable
```csharp

//select all
var student = db.Queryable<Student>().ToList();
var studentDynamic = db.Queryable<Student>().ToDynamic();
var studentJson = db.Queryable<Student>().ToJson();


//select single
var single = db.Queryable<Student>().Single(c => c.id == 1);
//select single by primarykey
var singleByPk = db.Queryable<Student>().InSingle(1);
//select single or default
var singleOrDefault = db.Queryable<Student>().SingleOrDefault(c => c.id == 11111111);
//select single or default
var single2 = db.Queryable<Student>().Where(c => c.id == 1).SingleOrDefault();

//select first
var first = db.Queryable<Student>().Where(c => c.id == 1).First();
var first2 = db.Queryable<Student>().Where(c => c.id == 1).FirstOrDefault();

//between 11 and 20
var page1 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(10).Take(10).ToList();

//between 11 and 20 
var page2 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).ToPageList(2, 10);

//get count
var count = db.Queryable<Student>().Where(c => c.id > 10).Count();

//skip 2
var skip = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(2).ToList();

//take 2
var take = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Take(2).ToList();

//Not like 
string conval = "a";
var notLike = db.Queryable<Student>().Where(c => !c.name.Contains(conval.ToString())).ToList();

//Like
conval = "三";
var like = db.Queryable<Student>().Where(c => c.name.Contains(conval)).ToList();

//where sql string
var student12 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>@id", new { id = 1 }).ToList();
var student13 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100 and id in( select 1)").ToList();


//is any
bool isAny100 = db.Queryable<Student>().Any(c => c.id == 100);
bool isAny1 = db.Queryable<Student>().Any(c => c.id == 1);


//get max id
object maxId = db.Queryable<Student>().Max(it => it.id);
int maxId1 = db.Queryable<Student>().Max(it => it.id).ObjToInt();
int maxId2 = db.Queryable<Student>().Max<int>("id"); 

//get min id
int minId1 = db.Queryable<Student>().Where(c => c.id > 0).Min(it => it.id).ObjToInt();
int minId2 = db.Queryable<Student>().Where(c => c.id > 0).Min<int>("id");


//order By 
var orderList = db.Queryable<Student>().OrderBy("id desc,name asc").ToList();
//order by 
var order2List = db.Queryable<Student>().OrderBy(it => it.name).OrderBy(it => it.id, OrderByType.desc).ToList(); // order by name as ,order by id desc

//In
var intArray = new[] { "5", "2", "3" };
var intList = intArray.ToList();
var listnew = db.Queryable<Student>().Where(it => intArray.Contains(it.name)).ToList();
var list0 = db.Queryable<Student>().In(it => it.id, 1, 2, 3).ToList();
var list1 = db.Queryable<Student>().In(it => it.id, intArray).ToList();
var list2 = db.Queryable<Student>().In("id", intArray).ToList();
var list3 = db.Queryable<Student>().In(it => it.id, intList).ToList();
var list4 = db.Queryable<Student>().In("id", intList).ToList();
var list6 = db.Queryable<Student>().In(intList).ToList(); //in primary fileds

//group by
var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,count(*) Count").ToDynamic();
var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it => it.id).Select("id,sex,count(*) Count").ToDynamic();
List<StudentGroup> list9 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<StudentGroup>("Sex,count(*) Count").ToList();
List<StudentGroup> list10 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<StudentGroup>("Sex,count(*) Count").ToList();
//SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex --生成结果



//join
var jList = db.Queryable<Student>()
	.JoinTable<School>((s1, s2) => s1.sch_id == s2.id) //默认left join
	.Where<School>((s1, s2) => s1.id == 1)
	.Select("s1.*,s2.name as schName")
	.ToDynamic();

/*join sql
						 SELECT s1.*,s2.name as schName 
						 FROM [Student]  s1 
						 LEFT JOIN [School]  s2 ON  s1.sch_id  = s2.id 
						 WHERE  s1.id  = 1 */

//join and page
var jList2 = db.Queryable<Student>()
	.JoinTable<School>((s1, s2) => s1.sch_id == s2.id) //默认left join
	//left inner join 
	//.JoinTable<School>((s1, s2) => s1.sch_id == s2.id ,JoinType.INNER)
	.Where<School>((s1, s2) => s1.id > 1)
	.OrderBy(s1 => s1.name)
	.Skip(10)
	.Take(20)
	.Select("s1.*,s2.name as schName")
	.ToDynamic();

//join three tables
var jList3 = db.Queryable<Student>()
	.JoinTable<School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
	.JoinTable<School>((s1, s3) => s1.sch_id == s3.id) // left join  School s3  on s1.id=s3.id
	.Where<School>((s1, s2) => s1.id > 1)  // where s1.id>1
	.Where(s1 => s1.id > 0)
	.OrderBy<School>((s1, s2) => s1.id) 
	.Skip(10)
	.Take(20)
	.Select("s1.*,s2.name as schName,s3.name as schName2")//select string
	.ToDynamic();


//join five
List<V_Student> jList4 =
	db.Queryable<Student>()
	.JoinTable<School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
	.JoinTable<School, Area>((s1, s2, a1) => a1.id == s2.AreaId)// left join  Area a1  on a1.id=s2.AreaId  
		.JoinTable<Area, School>((s1, a1, s3) => a1.id == s3.AreaId)// left join  School s3  on a1.id=s3.AreaId  
			.JoinTable<School>((s1, s4) => s1.sch_id == s4.id) // left join  School s2  on s1.id=s4.id
			.Select<School, Area, V_Student>((s1, s2, a1) => new V_Student { id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name }).ToList();

//join five sql
//SELECT id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name  
//FROM [Student]   s1 
//LEFT JOIN School  s2 ON  ( s1.sch_id  = s2.id )    
//LEFT JOIN Area  a1 ON  ( a1.id  = s2.AreaId )     
//LEFT JOIN School  s3 ON  ( a1.id  = s3.AreaId )    
//LEFT JOIN School  s4 ON  ( s1.sch_id  = s4.id )    
//WHERE 1=1    


//Join child
var childQuery = db.Queryable<Area>().Where("id=@id").Select(it => new { id = it.id }).ToSql();//create child SQL
string childTableName =SqlSugarTool.PackagingSQL(childQuery.Key);//package sql
var queryable = db.Queryable<Student>()
	.JoinTable<School>((s1, s2) => s1.sch_id == s2.id)  //LEFT JOIN School  s2 ON  ( s1.sch_id  = s2.id )  
	.JoinTable(childTableName, "a1", "a1.id=s2.areaid", new { id = 1 }, JoinType.INNER) //INNER JOIN (SELECT *  FROM [Area]   WHERE 1=1  AND id=@id   ) a1 ON a1.id=s2.areaid
	.OrderBy(s1 => s1.id);

var list = queryable.Select<School, Area, V_Student>((s1, s2, a1) => new V_Student { id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name })
	.ToPageList(0, 200);
var count2 = queryable.Count();


//append queryable
var queryable2 = db.Queryable<Student>().Where(it => true);
if (maxId.ObjToInt() == 1)
{
	queryable2.Where(it => it.id == 1);
}
else
{
	queryable2.Where(it => it.id == 2);
}
var listJoin = queryable2.ToList();


//queryable SqlSugarClient 
var par = new Queryable<Student>().Where(it => it.id == 1);//声名没有connection对象的Queryable
par.DB = db;
var listPar = par.ToList();


//get sql和 pars
var id = 1;
var sqlAndPars = db.Queryable<Student>().Where(it => it.id == id).OrderBy(it => it.id).ToSql();



//express functions
var par1 = "2015-1-1"; var par2 = "   I have a trim  ";
var r1 = db.Queryable<Student>().Where(it => it.name == par1.ObjToString()).ToList(); //ObjToString if null return ""
var r2 = db.Queryable<InsertTest>().Where(it => it.d1 == par1.ObjToDate()).ToList();
var r3 = db.Queryable<InsertTest>().Where(it => it.id == 1.ObjToInt()).ToList();//ObjToInt if null return 0
var r4 = db.Queryable<InsertTest>().Where(it => it.id == 2.ObjToDecimal()).ToList();
var r5 = db.Queryable<InsertTest>().Where(it => it.id == 3.ObjToMoney()).ToList();
var r6 = db.Queryable<InsertTest>().Where(it => it.v1 == par2.Trim()).ToList();
var convert1 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();
var convert2 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();
var convert3 = db.Queryable<Student>().Where(c => c.name == par2.ToLower()).ToList();
var convert4 = db.Queryable<Student>().Where(c => c.name == par2.ToUpper()).ToList();
var convert5= db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();
var c1 = db.Queryable<Student>().Where(c => c.name.Contains("a")).ToList();
var c2 = db.Queryable<Student>().Where(c => c.name.StartsWith("a")).ToList();
var c3 = db.Queryable<Student>().Where(c => c.name.EndsWith("a")).ToList();
var c4 = db.Queryable<Student>().Where(c => !string.IsNullOrEmpty(c.name)).ToList();
var c5 = db.Queryable<Student>().Where(c => c.name.Equals("小杰")).ToList();
var c6 = db.Queryable<Student>().Where(c => c.name.Length > 4).ToList();
var time = db.Queryable<InsertTest>().Where(c => c.d1>DateTime.Now.AddDays(1)).ToList();
var time2 = db.Queryable<InsertTest>().Where(c => c.d1 > DateTime.Now.AddYears(1)).ToList();
var time3 = db.Queryable<InsertTest>().Where(c => c.d1 > DateTime.Now.AddMonths(1)).ToList();
var intList = intArray.ToList();
var list0 = db.Queryable<Student>().In(it => it.id, 1,2,3).ToList();
var list1 = db.Queryable<Student>().In(it=>it.id, intArray).ToList();
var list2 = db.Queryable<Student>().In("id", intArray).ToList();
var list3 = db.Queryable<Student>().In(it => it.id, intList).ToList();
var list4 = db.Queryable<Student>().In("id", intList).ToList();

//group by
var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,Count=count(*)").ToDynamic();
var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it=>it.id).Select("id,sex,Count=count(*)").ToDynamic();
List<SexTotal> list5 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
List<SexTotal> list6 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<Student, SexTotal>    ("Sex,Count=count(*)").ToList();
//SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex 


//join
var jList = db.Queryable<Student>()
	.JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) //detault left join
		.Where<Student, School>((s1, s2) => s1.id == 1)
			.Select("s1.*,s2.name as schName")
			.ToDynamic();

//join by page
var jList2 = db.Queryable<Student>()
	.JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) //default left join
		//inner join
		//.JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id  ,JoinType.INNER)
		.Where<Student, School>((s1, s2) => s1.id > 1)
			.OrderBy<Student, School>((s1, s2) => s1.name)
				.Skip(10)
				.Take(20)
				.Select("s1.*,s2.name as schName")
				.ToDynamic();

//join select new 
var jList3 = db.Queryable<Student>()
	.JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
		.Where<Student, School>((s1, s2) => s1.id > 1)  // where s1.id>1
			.OrderBy<Student, School>((s1, s2) => s1.id) //order by s1.id no one  ordder   .oderBy().orderby  
				.Skip(1)
				.Take(2)
				.Select<Student, School, classNew>((s1, s2) => new classNew() { newid = s1.id, newname = s2.name, xx_name = s1.name }) 
					.ToList();
```

1.2 SqlQuery
```csharp
//to list
List<Student> list1 = db.SqlQuery<Student>("select * from Student");
//to list with par
List<Student> list2 = db.SqlQuery<Student>("select * from Student where id=@id", new { id = 1 });
//to dynamic
dynamic list3 = db.SqlQueryDynamic("select * from student");
//to json
string list4 = db.SqlQueryJson("select * from student");
//get int
var list5 = db.SqlQuery<int>("select top 1 id from Student").SingleOrDefault();
//get dictionary
Dictionary<string, string> list6 = db.SqlQuery<KeyValuePair<string, string>>("select id,name from Student").ToDictionary(it => it.Key, it => it.Value);
//get List<string[]>
var list7 = db.SqlQuery<string[]>("select top 1 id,name from Student").SingleOrDefault();
//get sp result
var spResult = db.SqlQuery<School>("exec sp_school @p1,@p2", new { p1 = 1, p2 = 2 });

//get sp and output 
var pars = SqlSugarTool.GetParameters(new { p1 = 1,p2=0 }); //dynmaic to SqlParameter
db.IsClearParameters = false;//close clear parametrs
pars[1].Direction = ParameterDirection.Output; //set  output
var spResult2 = db.SqlQuery<School>("exec sp_school @p1,@p2 output", pars);
db.IsClearParameters = true;//open  clear parameters
var outPutValue = pars[1].Value;//get output @p2 value

//sp 
var pars2 = SqlSugarTool.GetParameters(new { p1 = 1, p2 = 0 }); 
db.CommandType = CommandType.StoredProcedure;
var spResult3 = db.SqlQuery<School>("sp_school", pars2);
db.CommandType = CommandType.Text;


//get first row first column
string v1 = db.GetString("select '张三' as name");
int v2 = db.GetInt("select 1 as name");
double v3 = db.GetDouble("select 1 as name");
decimal v4 = db.GetDecimal("select 1 as name");
//....
```
##### 1.3 Sqlable
```csharp
//join
List<School> dataList = db.Sqlable()
	.From("school", "s")
	.Join("student", "st", "st.id", "s.id", JoinType.INNER)
	.Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
	.Where("s.id>100 and s.id<@id")
	.Where("1=1")
	.OrderBy("id")
	.SelectToList<School/*new model*/>("st.*", new { id = 1 });

//join page
List<School> dataPageList = db.Sqlable()
	.From("school", "s")
	.Join("student", "st", "st.id", "s.id", JoinType.INNER)
	.Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
	.Where("s.id>100 and s.id<100")
	.SelectToPageList<School>("st.*", "s.id", 1, 10);

//page where
List<School> dataPageList2 = db.Sqlable()
	.From("school", "s")
	.Join("student", "st", "st.id", "s.id", JoinType.INNER)
	.Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
	.Where("s.id>100 and s.id<100 and s.id in (select 1 )" )
	.SelectToPageList<School>("st.*", "s.id", 1, 10);



//-------- Dynmaic OR Json-----//

//join
var list1 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDynamic("*", new { id = 1 });
var list2 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToJson("*", new { id = 1 });
var list3 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDataTable("*", new { id = 1 });

//page
var list4 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });
var list5 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageTable("s.*", "l.id", 1, 10, new { id = 1 });
var list6 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });


//--------append sqlable-----//
Sqlable sable = db.Sqlable().From<Student>("s").Join<School>("l", "s.sch_id", "l.id", JoinType.INNER);
string name = "a";
int id = 1;
if (!string.IsNullOrEmpty(name))
{
	sable = sable.Where("s.name=@name");
}
if (!string.IsNullOrEmpty(name))
{
	sable = sable.Where("s.id=@id or s.id=100");
}
if (id > 0)
{
	sable = sable.Where("l.id in (select top 10 id from school)");
}
var pars = new { id = id, name = name };
int pageCount = sable.Count(pars);
var list7 = sable.SelectToPageList<Student>("s.*", "l.id desc", 1, 20, pars);
```

# 2.Insert
````csharp
//insert item
db.Insert(GetInsertItem());  

//insert list
db.InsertRange(GetInsertList()); 

//insert list (so fast)
db.SqlBulkCopy(GetInsertList()); 


//setting disable insert columns
db.DisableInsertColumns = new string[] { "sex" };
Student s = new Student()
{
	name = "mr" + new Random().Next(1, int.MaxValue),
	sex = "gril"
};

var id = db.Insert(s); // insert  with no 【sex】

````

# 3.Update 
```csharp
//update specified column
db.Update<School>(new { name = "蓝翔14" }, it => it.id == 14); //only update name
db.Update<School, int>(new { name = "蓝翔11 23 12", areaId = 2 }, 11, 23, 12);
db.Update<School, string>(new { name = "蓝翔2" }, new string[] { "11", "21" });
db.Update<School>(new { name = "蓝翔2" }, it => it.id == 100);
var array=new int[]{1,2,3};
db.Update<School>(new { name = "蓝翔2" }, it => array.Contains(it.id));// id in 1,2,3

//update list  by enity primary key
var updateResult = db.UpdateRange(GetUpdateList());a

//update list  by enity primary key (so fast)
var updateResult2 = db.SqlBulkReplace(GetUpdateList2());

//update by dictionary
var dic = new Dictionary<string, string>();
dic.Add("name", "第十三条");
dic.Add("areaId", "1");
db.Update<School, int>(dic, 13);


//update  by  entity
db.Update(new School { id = 16, name = "蓝翔16", AreaId = 1 });
db.Update<School>(new School { id = 12, name = "蓝翔12", AreaId = 2 }, it => it.id == 18);
db.Update<School>(new School() { id = 11, name = "青鸟11" });

//settig disable Update Columns
db.DisableUpdateColumns = new string[] { "CreateTime" };//CreateTime no update

TestUpdateColumns updObj = new TestUpdateColumns()
{
	VGUID = Guid.Parse("542b5a27-6984-47c7-a8ee-359e483c8470"),
	Name = "xx",
	Name2 = "xx2",
	IdentityField = 0,
	CreateTime = null
};

db.Update(updObj);


```

# 4.Delete
```csharp
//delete by primary key
db.Delete<School, int>(10);

//delete by exp
db.Delete<School>(it => it.id > 100);//support it=>array.contains(it.id)

//delete by primary key values
db.Delete<School, string>(new string[] { "100", "101", "102" });

//delete by field values
db.Delete<School, string>(it => it.name, new string[] { "" });
db.Delete<School, int>(it => it.id, new int[] { 20, 22 });


//delete by entity
db.Delete(new School() { id = 200 });

//delete by sql where  string
db.Delete<School>("id=@id", new { id = 100 });

//false delete
//db.FalseDelete<school>("is_del", 100);
//sql: update school set is_del=1 where id in(100)
//db.FalseDelete<school>("is_del", it=>it.id==100);

```

# 5.Tran
```csharp
using (SqlSugarClient db = SugarDao.GetInstance())
{
	db.IsNoLock = true; 
	db.CommandTimeOut = 30000; 
	try
	{
		db.BeginTran();
		//db.BeginTran(IsolationLevel.ReadCommitted);+3

		db.CommitTran();
	}
	catch (Exception)
	{
		db.RollbackTran();
		throw;
	}
}
```

# 6.Rename tables
```csharp
public class MappingTable : IDemos
{

	public void Init()
	{
		Console.WriteLine("启动MappingTable.Init");

		//单个设置
		using (var db = SugarDao.GetInstance())
		{
			var list = db.Queryable<V_Student>("Student").ToList();//查询的是 select * from student 而我的实体名称为V_Student
		}


		//全局设置
		using (var db = SugarFactory.GetInstance())
		{
			var list = db.Queryable<V_Student>().ToList();//查询的是 select * from student 而我的实体名称为V_Student
		}
	}


	/// <summary>
	/// 全局配置类
	/// </summary>
	public class SugarConfigs
	{
		//key类名 value表名
		public static List<KeyValue> MpList = new List<KeyValue>(){
			new KeyValue(){ Key="FormAttr", Value="Flow_FormAttr"},
				new KeyValue(){ Key="Student3", Value="Student"},
				new KeyValue(){ Key="V_Student", Value="Student"}
		};
	}

	/// <summary>
	/// SqlSugar实例工厂
	/// </summary>
	public class SugarFactory
	{

		//禁止实例化
		private SugarFactory()
		{

		}
		public static SqlSugarClient GetInstance()
		{
			string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
			var db = new SqlSugarClient(connection);

			db.SetMappingTables(SugarConfigs.MpList);//设置关联表 (引用地址赋值，每次赋值都只是存储一个内存地址)



			//批量设置别名表
			//db.ClassGenerating.ForeachTables(db, tableName =>
			//{
			//    db.AddMappingTable(new KeyValue() { Key = tableName.Replace("bbs.",""), Value =  tableName }); //key实体名，value表名
			//});


			return db;
		}
	}
}
```

# 7.Rename Columns
```csharp
//别名列的功能
public class MappingColumns : IDemos
{

	public void Init()
	{
		Console.WriteLine("启动MappingColumns.Init");

		//全局设置
		using (var db = SugarFactory.GetInstance())
		{
			var list = db.Queryable<Student>().Where(it=>it.classId==1).ToList();
		}
	}

	public class Student
	{

		//id
		public int classId { get; set; }

		//name
		public string className { get; set; }

		//sch_id
		public int classSchoolId { get; set; }

		public int isOk { get; set; }
	}

	/// <summary>
	/// 全局配置别名列（不区分表）
	/// </summary>
	public class SugarConfigs
	{
		//key实体字段名 value表字段名 ，KEY唯一否则异常
		public static List<KeyValue> MpList = new List<KeyValue>(){
			new KeyValue(){ Key="classId", Value="id"},
				new KeyValue(){ Key="className", Value="name"},
				new KeyValue(){ Key="classSchoolId", Value="sch_id"}
		};
	}

	/// <summary>
	/// SqlSugar实例工厂
	/// </summary>
	public class SugarFactory
	{

		//禁止实例化
		private SugarFactory()
		{

		}
		public static SqlSugarClient GetInstance()
		{
			string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
			var db = new SqlSugarClient(connection);
			//注意：只有启动属性映射才可以使用SetMappingColumns
			db.IsEnableAttributeMapping = true;
			db.SetMappingColumns(SugarConfigs.MpList);//设置关联列 (引用地址赋值，每次赋值都只是存储一个内存地址)
			return db;
		}
	}
}
```

#8.Ignore error columns
```csharp
using (var db = SugarDao.GetInstance())
{
	db.IsIgnoreErrorColumns = true;
}
```

#9.Create class files
```csharp
db.ClassGenerating.CreateClassFiles(db, ("e:/TestModels"), "Models");
```

# More [http://www.cnblogs.com/sunkaixuan/p/5911334.html](http://www.cnblogs.com/sunkaixuan/p/5911334.html)
