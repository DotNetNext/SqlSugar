using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class NavContext
    {
       public List<NavContextItem> Items { get; set; }
    }
    internal class NavContextItem
    {
        public int Level { get; set; }
        public string RootName { get; set; }
    }
}
