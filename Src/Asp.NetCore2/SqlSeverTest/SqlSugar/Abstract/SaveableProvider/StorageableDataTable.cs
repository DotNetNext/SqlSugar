using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class StorageableDataTable
    {
        internal DataTable DataTable { get;  set; }
        internal SqlSugarProvider Context { get;  set; }
        internal string[] Columns { get; set; } = new string[] { };
        internal string SugarGroupId = "SugarGroupId";
        internal string SugarUpdateRows = "SugarUpdateRows";
        internal string SugarColumns = "SugarColumns";
        internal string SugarErrorMessage = "SugarErrorMessage";
        internal List<DataRow> dbDataList = new List<DataRow>();
        List<KeyValuePair<StorageType, Func<DataRow, bool>,string>> whereFuncs = new List<KeyValuePair<StorageType, Func<DataRow, bool>,string>>();
        public StorageableDataTable WhereColumns(string name)
        {
            return WhereColumns(new string[] { name});
        }
        public StorageableDataTable WhereColumns(string[] names)
        {
            this.Columns = names;
            var queryable = this.Context.Queryable<object>();
            Check.Exception(Columns==null|| Columns.Length==0,"need WhereColums");
            var tableName = queryable.SqlBuilder.GetTranslationTableName(DataTable.TableName);
            this.Context.Utilities.PageEach(DataTable.Rows.Cast<DataRow>(), 200, itemList =>
            {
                List<IConditionalModel> conditList = new List<IConditionalModel>();
                SetConditList(itemList, Columns, conditList);
                var addItem = this.Context.Queryable<object>().AS(tableName).Where(conditList).ToDataTable().Rows.Cast<DataRow>().ToList();
                this.dbDataList.AddRange(addItem);
            });
            return this;
        }
        public StorageableDataTable WhereColumns(List<string> names)
        {
            return WhereColumns(names.ToArray());
        }
        public StorageableDataTable SplitInsert(Func<DataRow, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<DataRow, bool>, string>(StorageType.Insert, conditions,message));
            return this;
        }
        public StorageableDataTable SplitDelete(Func<DataRow, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<DataRow, bool>,string>(StorageType.Delete, conditions,message));
            return this;
        }
        public StorageableDataTable SplitUpdate(Func<DataRow, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<DataRow, bool>,string>(StorageType.Update, conditions,message));
            return this;
        }

        public StorageableDataTable Saveable(string inserMessage = null, string updateMessage = null)
        {
            SplitUpdate(it => it.Any(), updateMessage);
            SplitInsert(it => true, inserMessage);
            return this;
        }
        public StorageableDataTable SplitError(Func<DataRow, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<DataRow, bool>, string>(StorageType.Error, conditions, message));
            return this;
        }
        public StorageableDataTable SplitIgnore(Func<DataRow, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<DataRow, bool>, string>(StorageType.Ignore, conditions, message));
            return this;
        }

        public DataTableResult ToStorage()
        {
            if (whereFuncs == null || whereFuncs.Count == 0)
            {
                Saveable();
            }
            foreach (DataRow row in DataTable.Rows)
            {
                foreach (var item in whereFuncs.OrderByDescending(it => (int)it.key))
                {
                    SplitMethod(item.value1,item.key,row,item.value2);
                }
                if (row[SugarGroupId] == null || row[SugarGroupId] == DBNull.Value) 
                {
                    row[SugarGroupId] = StorageType.Ignore;
                }
            }
            DataTable.Columns.Remove(SugarUpdateRows);
            DataTable.Columns.Remove(SugarColumns);
            var Groups=DataTable.Rows.Cast<DataRow>()
                .Where(it=> it[SugarGroupId]!=null&& it[SugarGroupId] != DBNull.Value)
                .GroupBy(it => ((StorageType)it[SugarGroupId]).ToString()).Select(it=>new DataTableGroups{ Type=it.Key,DataTable= it.CopyToDataTable() })
                .ToList();
            DataTable.Columns.Remove(SugarGroupId);
            DataTable.Columns.Remove(SugarErrorMessage);
            var inserList = new List<Dictionary<string, object>>();
            var updateList = new List<Dictionary<string, object>>();
            var DeleteList=Groups.FirstOrDefault(it=>it.Type==StorageType.Delete.ToString());
            if (Groups.Any(it => it.Type == StorageType.Insert.ToString())) 
            {
                foreach (var item in Groups)
                {
                    if (item.Type == StorageType.Insert.ToString()) 
                    {
                        item.DataTable.Columns.Remove(SugarGroupId);
                        item.DataTable.Columns.Remove(SugarErrorMessage);
                        inserList.AddRange(this.Context.Utilities.DataTableToDictionaryList(item.DataTable));
                    }
                }
            }
            if (Groups.Any(it => it.Type == StorageType.Update.ToString()))
            {
                foreach (var item in Groups)
                {
                    if (item.Type == StorageType.Update.ToString())
                    {
                        item.DataTable.Columns.Remove(SugarGroupId);
                        item.DataTable.Columns.Remove(SugarErrorMessage);
                        updateList.AddRange(this.Context.Utilities.DataTableToDictionaryList(item.DataTable));
                    }
                }
            }
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            if (DeleteList!=null) 
            {
                SetConditList(DeleteList.DataTable.Rows.Cast<DataRow>().ToList(), Columns, conditionalModels);
            }
            var tableName = this.Context.Queryable<object>().SqlBuilder.GetTranslationTableName(DataTable.TableName);
            DataTableResult result = new DataTableResult() 
            { 
               DataTableGroups=Groups,
               AsDeleteable=this.Context.Deleteable<object>().AS(tableName).Where(conditionalModels),
               AsUpdateable= this.Context.Updateable(updateList).AS(tableName).WhereColumns(Columns),
               AsInsertable=this.Context.Insertable(inserList).AS(tableName) 
            };
            return result;
        }

        private void SplitMethod(Func<DataRow, bool> conditions, StorageType type,DataRow item,string message)
        {
            item[SugarColumns] = Columns;
            item[SugarUpdateRows] = dbDataList;
            if ((item[SugarGroupId]==null|| item[SugarGroupId] == DBNull.Value) && conditions(item))
            {
                item[SugarGroupId] = type;
                item[SugarErrorMessage] = message;
            }
        }
        private void SetConditList(List<DataRow> itemList, string[] whereColumns, List<IConditionalModel> conditList)
        {
            ;
            foreach (var dataItem in itemList)
            {
                var condition = new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                };
                conditList.Add(condition);
                int i = 0;
                foreach (var name in whereColumns)
                {
                    var value = dataItem[name];
                    if (value != null && value.GetType().IsEnum())
                    {
                        value = Convert.ToInt64(value);
                    }
                    condition.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(i == 0 ? WhereType.Or : WhereType.And, new ConditionalModel()
                    {
                        FieldName = name,
                        ConditionalType = ConditionalType.Equal,
                        FieldValue = value + "",
                        FieldValueConvertFunc = this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL ?
                                               UtilMethods.GetTypeConvert(value) : null
                    }));
                    ++i;
                }
            }
        }
    }

    public class DataTableResult 
    {
        public List<DataTableGroups> DataTableGroups { get; set; }
        public IUpdateable<Dictionary<string, object>> AsUpdateable { get; set; }
        public IDeleteable<object> AsDeleteable { get; set; }
        public IInsertable<Dictionary<string, object>> AsInsertable { get; set; }
    }
    public class DataTableGroups
    {
        public string Type { get;  set; }
        public DataTable DataTable { get;  set; }
    }
    public static class StorageableDataTableExtensions 
    {
        public static bool Any(this DataRow row) 
        {
            var list=row["SugarUpdateRows"] as List<DataRow>;
            var columns = row["SugarColumns"] as string[];
            return list.Any(it =>
            {
                var result = true;
                foreach (var name in columns)
                {

                    if (result)
                    {
                        result = row[name].ObjToString() == it[name].ObjToString();
                        if (result == false) 
                        {
                            break;
                        }
                    }
                }
                return result;
            });
        }
    }
}
