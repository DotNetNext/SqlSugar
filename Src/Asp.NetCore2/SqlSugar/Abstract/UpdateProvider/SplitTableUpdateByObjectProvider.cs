﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitTableUpdateByObjectProvider<T> where T : class, new()
    {
        public SqlSugarProvider Context;
        public UpdateableProvider<T> updateobj;
        public T[] UpdateObjects { get; set; }

        public IEnumerable<SplitTableInfo> Tables { get; set; }
        internal List<string> WhereColumns {   get;   set; }
        internal bool IsEnableDiffLogEvent { get;  set; }
        internal object BusinessData { get;   set; }

        public int ExecuteCommandWithOptLock(bool isThrowError = false) 
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                if (IsVersion())
                {
                    Check.ExceptionEasy(addList.Count > 1, "The version number can only be used for single record updates", "版本号只能用于单条记录更新");
                    result += this.Context.Updateable(addList.First())
                    .WhereColumns(this.WhereColumns?.ToArray())
                    .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                    .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                    .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity, this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                    .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommandWithOptLock(isThrowError);
                }
                else
                {
                    result += this.Context.Updateable(addList)
                        .WhereColumns(this.WhereColumns?.ToArray())
                        .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                        .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                        .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity, this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                        .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommandWithOptLock(isThrowError);
                }
            }
            return result;
        }
        public int ExecuteCommand()
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += this.Context.Updateable(addList)
                    .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                    .WhereColumns(this.WhereColumns?.ToArray())
                    .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                    .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity,this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                    .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommand();
            }
            return result;
        }


        public async Task<int> ExecuteCommandAsync()
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += await this.Context.Updateable(addList)
                    .WhereColumns(this.WhereColumns?.ToArray())
                    .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                    .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                    .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity, this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                    .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommandAsync();
            }
            return result;
        }
        public async Task<int> ExecuteCommandWithOptLockAsync(bool isThrowError = false) 
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                if (IsVersion())
                {
                    Check.ExceptionEasy(addList.Count > 1, "The version number can only be used for single record updates", "版本号只能用于单条记录更新");
                    result += await this.Context.Updateable(addList.First())
                      .WhereColumns(this.WhereColumns?.ToArray())
                      .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                      .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                      .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity, this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                      .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommandWithOptLockAsync(isThrowError);

                }
                else
                {
                    result += await this.Context.Updateable(addList)
                        .WhereColumns(this.WhereColumns?.ToArray())
                        .EnableDiffLogEventIF(this.IsEnableDiffLogEvent, this.BusinessData)
                        .UpdateColumns(updateobj.UpdateBuilder.UpdateColumns?.ToArray())
                        .IgnoreColumns(this.updateobj.UpdateBuilder.IsNoUpdateNull, this.updateobj.UpdateBuilder.IsOffIdentity, this.updateobj.UpdateBuilder.IsNoUpdateDefaultValue)
                        .IgnoreColumns(GetIgnoreColumns()).AS(item.Key).ExecuteCommandWithOptLockAsync(isThrowError);
                }
            }
            return result;
        }
        private string [] GetIgnoreColumns()
        {
            if (this.updateobj.UpdateBuilder.DbColumnInfoList.Any())
            {
               var columns=this.updateobj.UpdateBuilder.DbColumnInfoList.Select(it => it.DbColumnName).Distinct().ToList();
               var result= this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(x => !columns.Any(y => y.EqualCase(x.DbColumnName))).Select(it => it.DbColumnName).ToArray();
               result = result.Where(it => !string.IsNullOrEmpty(it)).ToArray();
               return result;
            }
            else 
            {
                return null;
            }
        }
        private void GroupDataList(T[] datas, out List<GroupModel> groupModels, out int result)
        {
            var attribute = typeof(T).GetCustomAttribute<SplitTableAttribute>() as SplitTableAttribute;
            Check.Exception(attribute == null, $"{typeof(T).Name} need SplitTableAttribute");
            groupModels = new List<GroupModel>();
            var db = this.Context;
            foreach (var item in datas)
            {
                var value = db.SplitHelper<T>().GetValue(attribute.SplitType, item);
                var tableName = db.SplitHelper<T>().GetTableName(attribute.SplitType, value);
                groupModels.Add(new GroupModel() { GroupName = tableName, Item = item });
            }
            result = 0;
        }
        private bool IsVersion()
        {
            return this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Any(it => it.IsEnableUpdateVersionValidation);
        }

        internal class GroupModel
        {
            public string GroupName { get; set; }
            public T Item { get; set; }
        }
    }
}
