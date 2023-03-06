using SqlSugar;
using System.Collections.Generic;
using System.Security.Principal;

namespace Test.Model
{
    [SugarTable("SchoolA01")]
    public class School
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int? Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "PId")]
        public int? PId { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Room.PId))]
        public List<Room> Rooms { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Playground.PId))]
        public List<Playground> Playgrounds { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(OtherThingOne.PId))]
        public List<OtherThingOne> OtherThingOnes { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(OtherThingTwo.PId))]
        public List<OtherThingTwo> OtherThingTwos { get; set; }
    }
    [SugarTable("OtherThingOne")]
    public class OtherThingOne
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int  Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "PId")]
        public int? PId { get; set; }
    }
    [SugarTable("OtherThingTwo")]
    public class OtherThingTwo
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int  Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string  Name { get; set; }

        [SugarColumn(ColumnName = "PId")]
        public int? PId { get; set; }
    }
}
