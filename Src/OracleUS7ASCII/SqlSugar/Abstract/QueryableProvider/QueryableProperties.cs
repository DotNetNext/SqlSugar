using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public SqlSugarProvider Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public MappingTableList QueryableMappingTableList { get; set; }
        public List<Action<T>> MapperAction { get; set; }
        public Action<T, MapperCache<T>> MapperActionWithCache { get; set; }
        public List<Action<List<T>>> Mappers { get; set; }
        public bool IsCache { get; set; }
        public int CacheTime { get; set; }
        public string CacheKey { get; set; }
        public bool IsAs { get; set; }
        public QueryBuilder QueryBuilder { get{ return this.SqlBuilder.QueryBuilder;} set{ this.SqlBuilder.QueryBuilder = value;}}
        public EntityInfo EntityInfo{get{ return this.Context.EntityMaintenance.GetEntityInfo<T>();}}
    }
}
