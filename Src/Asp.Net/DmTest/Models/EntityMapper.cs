using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    [SugarTable("MyEntityMapper")]
    public class EntityMapper
    {
        [SugarColumn(ColumnName ="MyName")]
        public string Name { get; set; }
    }
}
