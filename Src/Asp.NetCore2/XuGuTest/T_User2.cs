using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    [SqlSugar.SugarTable("T_User222")]
    public class T_User2
    {
        [SqlSugar.SugarColumn(DefaultValue = "0", ColumnDescription = "aaa")]
        public int number { get; set; }
    }
}
