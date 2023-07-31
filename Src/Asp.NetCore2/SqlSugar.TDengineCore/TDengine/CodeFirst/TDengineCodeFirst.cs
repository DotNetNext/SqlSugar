using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.TDengine
{
    public class TDengineCodeFirst : CodeFirstProvider
    {  
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            throw  new NotSupportedException("TDengine 暂时不支持CodeFirst等方法还在开发");
        } 
    }
}
