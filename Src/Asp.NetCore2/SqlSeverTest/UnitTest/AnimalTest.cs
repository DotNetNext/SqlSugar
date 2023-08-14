using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrmTest;
using SqlSugar;
namespace OrmTest 
{
    internal class AnimalTest
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Animal,Dog, Cat>();
            db.DbMaintenance.TruncateTable<Animal>();
            var dog = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
            db.Insertable(dog).ExecuteCommand();
            var cat = new Cat { Name = "Whiskers", Color = "Gray" };
            db.Insertable(cat).ExecuteCommand();

            var catList=db.Queryable<Cat>().ToList();
            var dogList = db.Queryable<Dog>().ToList();
            var dt=db.Queryable<Animal>().Select("*").ToDataTable();
            if (catList.Count() != 1 || dogList.Count() != 1 || dt.Rows.Count != 2) 
            {
                throw new Exception("unit error");
            }
            if (catList.First().Color!= "Gray"|| dogList.First().Breed != "Golden Retriever")
            {
                throw new Exception("unit error");
            }
            var xx = db.DbMaintenance.IsAnyColumnRemark("Id", "DBO.Order");
            if (xx)
            {
                db.DbMaintenance.DeleteColumnRemark("Id", "DBO.Order");
            }
            db.Queryable<Dog>().MergeTable().LeftJoin<Order>((x, y) => x.DogId == y.Id)
                .ToList();

            db.QueryFilter.AddTableFilter<Dog>(x => x.DogId == 2);
            db.Queryable<Dog>()
                .LeftJoin<Dog>((x, y) => x.DogId == y.DogId)
                    .LeftJoin<Dog>((x, y,z) => x.DogId == z.DogId)
             .ToList();
            db.Queryable<Dog, Cat>((d, c) => new JoinQueryInfos(
                 JoinType.Left, d.AnimalId == c.AnimalId
                ))
             .ToList();
            db.Queryable<Dog, Cat,Cat>((d, c,cat) => new JoinQueryInfos(
              JoinType.Left, d.AnimalId == c.AnimalId,
               JoinType.Left, d.AnimalId == cat.AnimalId
             ))
            .ToList();
            db.DbMaintenance.AddColumnRemark("Id", "DBO.Order", "a");
        }
    }
    [SugarTable("Animal",IsDisabledDelete =true)]
    public class Animal
    {
        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int AnimalId { get; set; }
        public string Name { get; set; }
    }
    [SugarTable("Animal",Discrimator ="Type:1", IsDisabledDelete = true)]
    public class Dog : Animal
    {
        [SugarColumn(IsNullable =true)]
        public int DogId { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Breed { get; set; }
    }
    [SugarTable("Animal", Discrimator = "Type:2", IsDisabledDelete = true)]
    public class Cat : Animal
    {
        [SugarColumn(IsNullable = true)]
        public int CatId { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Color { get; set; }
    }
}
