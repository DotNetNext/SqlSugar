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
    }
}
