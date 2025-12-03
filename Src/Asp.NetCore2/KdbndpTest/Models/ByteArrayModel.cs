using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbndpTest.Models
{
    [SqlSugar.SugarTable("ByteArrayModel02")] 
    
    public class ByteArrayModel
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public byte[] Bytes { get; set; }
    }
}
