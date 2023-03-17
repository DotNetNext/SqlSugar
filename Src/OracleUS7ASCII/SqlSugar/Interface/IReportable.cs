using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public interface  IReportable<T>
    {
        //IReportable<T> MakeUp(Func<T,object> auto);
        ISugarQueryable<T> ToQueryable();
        ISugarQueryable<SingleColumnEntity<Y>> ToQueryable<Y>();
    }
}
