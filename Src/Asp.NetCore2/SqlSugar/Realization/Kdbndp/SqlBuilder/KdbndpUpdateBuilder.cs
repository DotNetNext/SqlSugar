using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class KdbndpUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE  {1} {2} SET {0} FROM  ${{0}}  ";
            }
        }
        public override string SqlTemplateJoin
        {
            get
            {
                return @"            (VALUES
              {0}

            ) AS T ({2}) WHERE {1}
                 ";
            }
        }

        public override string SqlTemplateBatchUnion
        {
            get
            {
                return ",";
            }
        }

        public override object FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType)
                {
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
                    }
                    return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return FormatDateTimeOffset(value);
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    string bytesString = "0x" + BitConverter.ToString((byte[])value);
                    return bytesString;
                }
                else if (type.IsEnum())
                {
                    return Convert.ToInt64(value);
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }

        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, " Update List<T> need Primary key");
            int pageSize = 200;
            int pageIndex = 1;
            int totalRecord = groupList.Count;
            int pageCount = (totalRecord + pageSize - 1) / pageSize;
            StringBuilder batchUpdateSql = new StringBuilder();
            while (pageCount >= pageIndex)
            {
                StringBuilder updateTable = new StringBuilder();
                string setValues = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result = string.Format("{0}=T.{0}", Builder.GetTranslationColumnName(it.DbColumnName));
                    return result;
                }));
                string tempColumnValue = string.Join(",", groupList.First().Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result = Builder.GetTranslationColumnName(it.DbColumnName);
                    return result;
                }));
                batchUpdateSql.AppendFormat(SqlTemplateBatch.ToString(), setValues, GetTableNameStringNoWith, TableWithString);
                int i = 0;
                var tableColumnList = this.Context.DbMaintenance.GetColumnInfosByTableName(GetTableNameStringNoWith);
                
                foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                {
                    var isFirst = i == 0;
                    if (!isFirst)
                    {
                        updateTable.Append(SqlTemplateBatchUnion);
                    }
                    updateTable.Append("\r\n (" + string.Join(",", columns.Select(it =>
                    {
                        var columnInfo = tableColumnList.FirstOrDefault(x => x.DbColumnName.Equals(it.DbColumnName, StringComparison.OrdinalIgnoreCase));
                        var dbType = columnInfo?.DataType;
                        if (dbType == null) {
                            var typeName = it.PropertyType.Name.ToLower();
                            if (typeName == "int32")
                                typeName = "int";
                            if (typeName == "int64")
                                typeName = "long";
                            if (typeName == "int16")
                                typeName = "short";
                            if (typeName == "boolean")  
                                typeName = "bool";

                            var isAnyType = PostgreSQLDbBind.MappingTypesConst.Where(x => x.Value.ToString().ToLower() == typeName).Any();
                            if (isAnyType)
                            {
                                dbType = PostgreSQLDbBind.MappingTypesConst.Where(x => x.Value.ToString().ToLower() == typeName).FirstOrDefault().Key;
                            }
                            else {
                                dbType = "varchar";
                            }
                        }
                        return string.Format("CAST({0} AS {1})", base.GetDbColumn(it,FormatValue(it.Value)), dbType);

                    })) + ")");
                    ++i;
                }
                pageIndex++;
                updateTable.Append("\r\n");
                string whereString = null;
                if (this.WhereValues.HasValue())
                {
                    foreach (var item in WhereValues)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += item;
                    }
                }
                else if (PrimaryKeys.HasValue())
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += string.Format("{0}.{1}=T.{1}", GetTableNameStringNoWith, Builder.GetTranslationColumnName(item));
                    }
                }
                var format = string.Format(SqlTemplateJoin, updateTable, whereString, tempColumnValue);
                batchUpdateSql.Replace("${0}", format);
                batchUpdateSql.Append(";");
            }
            batchUpdateSql = GetBatchUpdateSql(batchUpdateSql);
            return batchUpdateSql.ToString();
        }

        private StringBuilder GetBatchUpdateSql(StringBuilder batchUpdateSql)
        {
            if (ReSetValueBySqlExpListType == null && ReSetValueBySqlExpList != null)
            {
                var result = batchUpdateSql.ToString();
                foreach (var item in ReSetValueBySqlExpList)
                {
                    var dbColumnName = item.Value.DbColumnName;
                    if (item.Value.Type == ReSetValueBySqlExpListModelType.List)
                    {
                        result = result.Replace($"{dbColumnName}=T.{dbColumnName}", $"{dbColumnName}={GetTableNameString}.{dbColumnName}{item.Value.Sql}T.{dbColumnName}");
                    }
                    else
                    {
                        result = result.Replace($"{dbColumnName}=T.{dbColumnName}", $"{dbColumnName}={item.Value.Sql.Replace(dbColumnName, $"{Builder.GetTranslationColumnName(this.TableName)}.{dbColumnName}")}");
                    }
                    batchUpdateSql = new StringBuilder(result);
                }
            }

            return batchUpdateSql;
        }
        protected override string GetJoinUpdate(string columnsString, ref string whereString)
        {
            var formString = $"  {Builder.GetTranslationColumnName(this.TableName)}  AS {Builder.GetTranslationColumnName(this.ShortName)} ";
            var joinString = "";
            foreach (var item in this.JoinInfos)
            {
                whereString += " AND " + item.JoinWhere;
                joinString += $"\r\n FROM {Builder.GetTranslationColumnName(item.TableName)}  {Builder.GetTranslationColumnName(item.ShortName)} ";
            }
            var tableName = formString + "\r\n ";
            columnsString = columnsString.Replace(Builder.GetTranslationColumnName(this.ShortName) + ".", "") + joinString;
            return string.Format(SqlTemplate, tableName, columnsString, whereString);
        }
        public override string FormatDateTimeOffset(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff zzz") + "'";
        }
    }
}
