using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class GuidPrimaryKey
    {
        public static void Init() 
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<Student>();
            var id = Guid.NewGuid();
            db.Insertable(new Student()
            {
                Name="a",
                Id=id
                 
            }).ExecuteCommand();

            var data=db.Queryable<Student>().Where(it => it.Id == id).ToList();
            if (data.First().Id != id) Cases.ThrowUnitError();
            data.First().Name = "a2";
            db.Updateable(data).ExecuteCommand();
            var data2 = db.Queryable<Student>().Where(it => it.Id == id).ToList();
            if (data.First().Name !="a2") Cases.ThrowUnitError();
            Console.WriteLine(db.Queryable<Student>().Count());
            db.Deleteable<Student>().Where(it=>it.Id==id).ExecuteCommand();
            if(db.Queryable<Student>().Count()!=0) Cases.ThrowUnitError();
            db.Insertable(new Student()
            {
                Name = "a" ,
                Id=id
            }).ExecuteCommand();
            var rows=db.Deleteable<Student>().Where(it => new Guid[] { id}.Contains(it.Id)).ExecuteCommand();
            if (db.Queryable<Student>().Count() != 0) Cases.ThrowUnitError();
            db.Insertable(new Student()
            {
                Name = "a",
                Id=id
            }).ExecuteCommand();
            var row2=db.Deleteable<Student>(new Student() { Id=id}).ExecuteCommand();
            if (db.Queryable<Student>().Count() != 0) Cases.ThrowUnitError();
        }

        [SqlSugar.SugarTable("UnitStutsdsdfr5fazzz1")]
        public class Student: MongoDbBaseGuid
        { 
            public string Name { get; set; }
        }
    }
}
