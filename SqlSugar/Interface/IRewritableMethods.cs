﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IRewritableMethods
    {
        ExpandoObject DataReaderToExpandoObject(IDataReader reader);
        List<T> DataReaderToDynamicList<T>(IDataReader reader);
        string SerializeObject(object value);
        T DeserializeObject<T>(string value);
        T TranslateCopy<T>(T sourceObject);
        dynamic DataTableToDynamic(DataTable table);
        ICacheManager<T> GetCacheInstance<T>();
     
    }
}
