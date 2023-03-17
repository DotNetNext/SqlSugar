using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar 
{
    public partial class InsertNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        private void InsertManyToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {;
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentPkColumn = parentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = thisEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            Check.ExceptionEasy(thisPkColumn == null, $"{thisPkColumn.EntityName} need primary key", $"{thisPkColumn.EntityName}需要主键");
            Check.ExceptionEasy(parentPkColumn == null, $"{parentPkColumn.EntityName} need primary key", $"{parentPkColumn.EntityName}需要主键");
            var mappingType=parentNavigateProperty.Navigat.MappingType;
            var mappingEntity = this._Context.EntityMaintenance.GetEntityInfo(mappingType);
            var mappingA = mappingEntity.Columns.FirstOrDefault(x=>x.PropertyName== parentNavigateProperty.Navigat.MappingAId);
            var mappingB = mappingEntity.Columns.FirstOrDefault(x => x.PropertyName == parentNavigateProperty.Navigat.MappingBId);
            Check.ExceptionEasy(mappingA == null || mappingB == null, $"Navigate property {name} error ", $"导航属性{name}配置错误");
            var mappingPk = mappingEntity.Columns
                   .Where(it => it.PropertyName != mappingA.PropertyName)
                   .Where(it => it.PropertyName != mappingB.PropertyName)
                   .Where(it => it.IsPrimarykey && !it.IsIdentity && it.OracleSequenceName.IsNullOrEmpty()).FirstOrDefault();
            var mappingOthers = mappingEntity.Columns
                   .Where(it => it.PropertyName != mappingA.PropertyName)
                   .Where(it => it.PropertyName != mappingB.PropertyName)
                   .Where(it => !it.IsIdentity)
                   .Where(it => !it.IsOnlyIgnoreInsert)
                   .Where(it => !it.IsIgnore)
                   .Where(it => !it.IsPrimarykey);
            List<Dictionary<string, object>> mappgingTables = new List<Dictionary<string, object>>();
            foreach (var item in parentList)
            {
                var items= parentNavigateProperty.PropertyInfo.GetValue(item);
                if (items == null)
                {
                    continue;
                }
                var children=((List<TChild>)items);
                InsertDatas(children, thisPkColumn);
                var parentId = parentPkColumn.PropertyInfo.GetValue(item);
                foreach (var child in children)
                {
                    var chidId = thisPkColumn.PropertyInfo.GetValue(child);
                    Dictionary<string,object> keyValuePairs = new Dictionary<string,object>();
                    keyValuePairs.Add(mappingA.DbColumnName, parentId);
                    keyValuePairs.Add(mappingB.DbColumnName, chidId);
                    if (mappingOthers != null)
                    {
                        foreach (var pair in mappingOthers)
                        {
                            if (!keyValuePairs.ContainsKey(pair.DbColumnName))
                            {
                                if (pair.UnderType == UtilConstants.DateType)
                                {
                                    keyValuePairs.Add(pair.DbColumnName, DateTime.Now);
                                }
                                else if (pair.UnderType == UtilConstants.StringType)
                                {
                                    keyValuePairs.Add(pair.DbColumnName, UtilConstants.Space);
                                }
                                else
                                {
                                    keyValuePairs.Add(pair.DbColumnName, UtilMethods.GetDefaultValue(pair.UnderType));
                                }
                            }
                        }
                    }
                    if (mappingPk != null) 
                    {
                        SetMappingTableDefaultValue(mappingPk, keyValuePairs);
                    }
                    mappgingTables.Add(keyValuePairs);
                }
            }
            var ids = mappgingTables.Select(x => x[mappingA.DbColumnName]).ToList();
            if (_navOptions != null && _navOptions.ManyToManyNoDeleteMap)
            {
                //The reserved
            }
            else
            {
                this._Context.Deleteable<object>().AS(mappingEntity.DbTableName).In(mappingA.DbColumnName, ids).ExecuteCommand();
            }
            this._Context.Insertable(mappgingTables).AS(mappingEntity.DbTableName).ExecuteCommand();
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }

        private void SetMappingTableDefaultValue(EntityColumnInfo mappingPk, Dictionary<string, object> keyValuePairs)
        {
            if (mappingPk.UnderType == UtilConstants.LongType)
            {
                keyValuePairs.Add(mappingPk.DbColumnName, SnowFlakeSingle.Instance.NextId());
            }
            else if (mappingPk.UnderType == UtilConstants.GuidType)
            {
                keyValuePairs.Add(mappingPk.DbColumnName, Guid.NewGuid());
            }
            else if (mappingPk.UnderType == UtilConstants.StringType)
            {
                keyValuePairs.Add(mappingPk.DbColumnName, Guid.NewGuid() + "");
            }
            else 
            {
                var name= mappingPk.EntityName+" "+ mappingPk.DbColumnName;    
                Check.ExceptionEasy($"The field {name} is not an autoassignment type and requires an assignment",
                    $" 中间表主键字段{name}不是可自动赋值类型， 可赋值类型有 自增、long、Guid、string。你也可以删掉主键 用双主键");
            }
        }
    }
}
