using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteNavOptions
    {
        public bool ManyToManyIsDeleteA { get; set; }
        public bool ManyToManyIsDeleteB { get; set; }
    }   
    public class UpdateNavOptions
    {
        public bool ManyToManyIsUpdateA { get; set; }
        public bool ManyToManyIsUpdateB { get; set; }
    }
}
