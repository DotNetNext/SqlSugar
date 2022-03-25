using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class LogicDeleteProvider<T> where T : class, new()
    {
        public DeleteableProvider<T> Deleteable { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public int ExecuteCommand(string LogicFieldName = null,object deleteValue=null,string deleteTimeFieldName = null)
        {
            ISqlSugarClient db;
            List<SugarParameter> pars;
            string where;
            LogicFieldName = _ExecuteCommand(LogicFieldName, out db, out where, out pars);
            if (deleteValue == null) 
            {
                deleteValue = true;
            }
            var updateable = db.Updateable<T>().SetColumns(LogicFieldName, deleteValue);
            if (deleteTimeFieldName != null)
            {
                updateable.SetColumns(deleteTimeFieldName, DateTime.Now);
            }
            if (pars != null)
                updateable.UpdateBuilder.Parameters.AddRange(pars);
            Convert(updateable as UpdateableProvider<T>);
            var result = updateable.Where(where).ExecuteCommand();
            return result;
        }
        public async Task<int> ExecuteCommandAsync(string LogicFieldName = null, object deleteValue = null, string deleteTimeFieldName = null)
        {
            ISqlSugarClient db;
            List<SugarParameter> pars;
            string where;
            LogicFieldName = _ExecuteCommand(LogicFieldName, out db, out where, out pars);
            if (deleteValue == null)
            {
                deleteValue = true;
            }
            var updateable = db.Updateable<T>().SetColumns(LogicFieldName, deleteValue);
            if (deleteTimeFieldName != null)
            {
                updateable.SetColumns(deleteTimeFieldName, DateTime.Now);
            }
            if (pars != null)
                updateable.UpdateBuilder.Parameters.AddRange(pars);
            Convert(updateable as UpdateableProvider<T>);
            var result =await updateable.Where(where).ExecuteCommandAsync();
            return result;
        }

        private void Convert(UpdateableProvider<T> updateable)
        {
            updateable.IsEnableDiffLogEvent = Deleteable.IsEnableDiffLogEvent;
            updateable.diffModel = Deleteable.diffModel;
            updateable.UpdateBuilder.TableWithString = DeleteBuilder.TableWithString;
            updateable.RemoveCacheFunc = Deleteable.RemoveCacheFunc;
        }

        private string _ExecuteCommand(string LogicFieldName, out ISqlSugarClient db, out string where, out List<SugarParameter> pars)
        {
            var entityInfo = Deleteable.EntityInfo;
            db = Deleteable.Context;
            where = DeleteBuilder.GetWhereString.Substring(5);
            pars = DeleteBuilder.Parameters;
            if (LogicFieldName.IsNullOrEmpty())
            {
                var column = entityInfo.Columns.FirstOrDefault(it =>
                it.PropertyName.EqualCase("isdelete") ||
                it.PropertyName.EqualCase("isdeleted") ||
                it.DbColumnName.EqualCase("isdelete") ||
                it.DbColumnName.EqualCase("isdeleted"));
                if (column != null)
                {
                    LogicFieldName = column.DbColumnName;
                }
            }
            Check.Exception(LogicFieldName == null, ErrorMessage.GetThrowMessage(
                 $"{entityInfo.EntityName} is not isdelete or isdeleted"
                , $"{entityInfo.EntityName} 没有IsDelete或者IsDeleted 的属性, 你也可以用 IsLogic().ExecuteCommand(\"列名\")"));
            return LogicFieldName;
        }
    }
}
