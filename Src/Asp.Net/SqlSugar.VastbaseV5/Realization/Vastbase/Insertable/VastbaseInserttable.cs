using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlSugar
{
    public class VastbaseV5Inserttable<T> : PostgreSQLInserttable<T> where T : class, new()
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public override int ExecuteReturnIdentity()
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                InsertBuilder.IsReturnIdentity = true;
                PreToSql();
                var sql = InsertBuilder.ToSqlString();
                RestoreMapping();
                var result = Ado.GetInt(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                After(sql, result);
                return result;
            }
            return base.ExecuteReturnIdentity();
        }

        public override async System.Threading.Tasks.Task<int> ExecuteReturnIdentityAsync()
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                InsertBuilder.IsReturnIdentity = true;
                PreToSql();
                var sql = InsertBuilder.ToSqlString();
                RestoreMapping();
                var result = await Ado.GetIntAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                After(sql, result);
                return result;
            }
            return await base.ExecuteReturnIdentityAsync();
        }

        public override SqlServerBlukCopy UseSqlServer()
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                if (this.InsertObjs != null && this.InsertObjs.Length > 0 && this.InsertObjs[0] is DataTable)
                {
                    var sourceDt = (DataTable)(object)this.InsertObjs[0];
                    // Babelfish：剥 identity 列走 auto-gen 默认（同 PG/SqlServer Fastest 默认）。SET IDENTITY_INSERT 已支持，但 Fastest 默认 auto-gen；opt-in 保留 identity 走 IsOffIdentity。
                    var dbCols = this.Context.DbMaintenance.GetColumnInfosByTableName(sourceDt.TableName);
                    if (dbCols != null)
                    {
                        var identityNames = dbCols.Where(c => c.IsIdentity).Select(c => c.DbColumnName).ToList();
                        foreach (var idCol in identityNames)
                        {
                            var colIndex = sourceDt.Columns.IndexOf(idCol);
                            if (colIndex >= 0)
                                sourceDt.Columns.RemoveAt(colIndex);
                        }
                    }
                    var fastBuilder = new VastbaseV5FastBuilder(null);
                    fastBuilder.Context = this.Context;
                    fastBuilder.ExecuteBulkCopyAsync(sourceDt).GetAwaiter().GetResult();
                    return new SqlServerBlukCopy();
                }
                PreToSql();
                var groupedList = InsertBuilder.DbColumnInfoList.GroupBy(it => it.TableId).ToList();
                if (groupedList.Count == 0)
                    return new SqlServerBlukCopy();
                var dt = new DataTable();
                dt.TableName = InsertBuilder.GetTableNameString;
                var entityCols = InsertBuilder.EntityInfo.Columns
                    .Where(c => c.DbColumnName != null)
                    .ToDictionary(c => c.DbColumnName);
                foreach (var col in groupedList.First())
                {
                    var type = typeof(object);
                    if (entityCols.ContainsKey(col.DbColumnName))
                    {
                        var propType = entityCols[col.DbColumnName].PropertyInfo?.PropertyType;
                        if (propType != null)
                        {
                            var underlying = Nullable.GetUnderlyingType(propType);
                            type = underlying ?? propType;
                        }
                    }
                    else
                    {
                        type = col.Value?.GetType() ?? typeof(object);
                    }
                    dt.Columns.Add(col.DbColumnName, type);
                }
                foreach (var rowGroup in groupedList)
                {
                    var dr = dt.NewRow();
                    foreach (var col in rowGroup)
                    {
                        var val = col.Value;
                        if (val is DateTime dtVal && dtVal < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            val = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                        dr[col.DbColumnName] = val ?? DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                }
                var fastBuilder2 = new VastbaseV5FastBuilder(null);
                fastBuilder2.Context = this.Context;
                fastBuilder2.ExecuteBulkCopyAsync(dt).GetAwaiter().GetResult();
                return new SqlServerBlukCopy();
            }
            return base.UseSqlServer();
        }
    }
}