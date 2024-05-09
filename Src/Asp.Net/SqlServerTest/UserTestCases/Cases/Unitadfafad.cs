using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfafad
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var list=db.Queryable<Order>()
                .Select(it => new Modelsdfa(it.Name, it.Id))
                .ToList();
        }
        public class Modelsdfa
        { 
            public Modelsdfa(string name,int id)
            {
                Name = name;
                Id = id;
            }

            public  string Name { get; set; } 
            public int Id { get; set; }
        }
    }
}
