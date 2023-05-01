using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        private void DeleteManyToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            var parentEntity = _ParentEntity;
            var parentList = _ParentList.Cast<T>().ToList();
            var parentPkColumn = parentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = thisEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            Check.ExceptionEasy(thisPkColumn == null, $"{thisPkColumn.EntityName} need primary key", $"{thisPkColumn.EntityName}需要主键");
            Check.ExceptionEasy(parentPkColumn == null, $"{parentPkColumn.EntityName} need primary key", $"{parentPkColumn.EntityName}需要主键");
            var mappingType = parentNavigateProperty.Navigat.MappingType;
            var mappingEntity = this._Context.EntityMaintenance.GetEntityInfo(mappingType);
            var mappingA = mappingEntity.Columns.FirstOrDefault(x => x.PropertyName == parentNavigateProperty.Navigat.MappingAId);
            var mappingB = mappingEntity.Columns.FirstOrDefault(x => x.PropertyName == parentNavigateProperty.Navigat.MappingBId);
            Check.ExceptionEasy(mappingA == null || mappingB == null, $"Navigate property {name} error ", $"导航属性{name}配置错误");
            var mappingPk = mappingEntity.Columns
                   .Where(it => it.PropertyName != mappingA.PropertyName)
                   .Where(it => it.PropertyName != mappingB.PropertyName)
                   .Where(it => it.IsPrimarykey && !it.IsIdentity && it.OracleSequenceName.IsNullOrEmpty()).FirstOrDefault();

            if (IsDeleteA())
            {
                if (!_IsDeletedParant)
                    SetContext(() => this._Context.Deleteable(parentList)
                    .EnableDiffLogEventIF(_RootOptions?.IsDiffLogEvent == true, _RootOptions?.DiffLogBizData)
                    .ExecuteCommand());
            }

            var aids = _ParentList.Select(it => parentPkColumn.PropertyInfo.GetValue(it)).ToList();
            var bids = _Context.Queryable<object>().Filter(mappingEntity.Type).AS(mappingEntity.DbTableName).In(mappingA.DbColumnName, aids)
                .Select(mappingB.DbColumnName).ToDataTable()
                .Rows.Cast<System.Data.DataRow>().Select(it => it[0]).ToList();


            var childList = GetChildList<TChild>().In(thisPkColumn.DbColumnName, bids).ToList();
            if (_WhereList.HasValue()) 
            {
                bids = childList.Select(it => thisPkColumn.PropertyInfo.GetValue(it)).ToList();
            }


            if (IsDeleteB())
            {
                SetContext(() => _Context.Deleteable(childList).ExecuteCommand());
            }

            this._ParentList = childList.Cast<object>().ToList();
            this._ParentPkColumn = thisPkColumn;
            this._IsDeletedParant = true;


            if (_WhereList.HasValue())
            {
                SetContext(() => _Context.Deleteable<object>().AS(mappingEntity.DbTableName)
                .In(mappingA.DbColumnName, aids)
                .In(mappingB.DbColumnName, bids)
                .ExecuteCommand());
            }
            else
            {
                SetContext(() => _Context.Deleteable<object>().AS(mappingEntity.DbTableName).In(
                    mappingA.DbColumnName, aids
                    ).ExecuteCommand());
            }

        }

        private bool IsDeleteA()
        {
            return deleteNavOptions != null && deleteNavOptions.ManyToManyIsDeleteA;
        }
        private bool IsDeleteB()
        {
            return deleteNavOptions != null && deleteNavOptions.ManyToManyIsDeleteB;
        }
    }
}
