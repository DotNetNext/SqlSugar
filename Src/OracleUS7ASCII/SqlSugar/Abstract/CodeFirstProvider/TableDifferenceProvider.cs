using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public class TableDifferenceProvider
    {
        internal List<DiffTableInfo> tableInfos = new List<DiffTableInfo>();
        public string ToDiffString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            var diffTables = this.ToDiffList();
            if (diffTables.IsNullOrEmpty())
            {
                sb.AppendLine("No change");
            }
            else
            {
                foreach (var item in diffTables)
                {
                    sb.AppendLine($"----Table:{ item.TableName }----");
                    if (item.AddColums.HasValue())
                    {
                        sb.AppendLine($"Add column: ");
                        foreach (var addItem in item.AddColums)
                        {
                            sb.AppendLine($"{addItem.Message} ");
                        }
                    }
                    if (item.UpdateColums.HasValue())
                    {
                        sb.AppendLine($"Update column: ");
                        foreach (var addItem in item.UpdateColums)
                        {
                            sb.AppendLine($"{addItem.Message} ");
                        }
                    }
                    if (item.DeleteColums.HasValue())
                    {
                        sb.AppendLine($"Delete column: ");
                        foreach (var addItem in item.DeleteColums) 
                        {
                            sb.AppendLine($"{addItem.Message} ");
                        }
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }

        public List<TableDifferenceInfo> ToDiffList()
        {
            List<TableDifferenceInfo> result = new List<TableDifferenceInfo>();
            foreach (var tableInfo in tableInfos)
            {
                TableDifferenceInfo addItem = new TableDifferenceInfo();
                if (tableInfo.OldTableInfo == null)
                    tableInfo.OldTableInfo = new DbTableInfo();
                addItem.TableName = tableInfo.OldTableInfo.Name;
                addItem.AddColums = GetAddColumn(tableInfo);
                addItem.UpdateColums = GetUpdateColumn(tableInfo);
                addItem.DeleteColums = GetDeleteColumn(tableInfo);
                if (addItem.IsDiff)
                    result.Add(addItem);
            }
            return result;
        }

        private static List<DiffColumsInfo> GetDeleteColumn(DiffTableInfo tableInfo)
        {
            List<DiffColumsInfo> result = new List<DiffColumsInfo>();
            var columns = tableInfo.OldColumnInfos.Where(z => !tableInfo.NewColumnInfos.Any(y => y.DbColumnName.EqualCase(z.DbColumnName))).ToList();
            return columns.Select(it => new DiffColumsInfo() {   Message= GetColumnString(it) }).ToList();
        }

        private List<DiffColumsInfo> GetUpdateColumn(DiffTableInfo tableInfo)
        {
            List<DiffColumsInfo> result = new List<DiffColumsInfo>();
            result = tableInfo.NewColumnInfos
                 .Where(z => tableInfo.OldColumnInfos.Any(y => y.DbColumnName.EqualCase(z.DbColumnName) && (
                     z.Length != y.Length ||
                     z.ColumnDescription != y.ColumnDescription ||
                     z.DataType != y.DataType ||
                     z.DecimalDigits != y.DecimalDigits
                  ))).Select(it => new DiffColumsInfo()
                  {
                     Message= GetUpdateColumnString(it, tableInfo.OldColumnInfos.FirstOrDefault(y => y.DbColumnName.EqualCase(it.DbColumnName)))
                  }).ToList();
            return result;
        }

        private static List<DiffColumsInfo> GetAddColumn(DiffTableInfo tableInfo)
        {
            List<DiffColumsInfo> result = new List<DiffColumsInfo>();
            var columns = tableInfo.NewColumnInfos.Where(z => !tableInfo.OldColumnInfos.Any(y => y.DbColumnName.EqualCase(z.DbColumnName))).ToList();
            return columns.Select(it => new DiffColumsInfo() {  Message = GetColumnString(it) }).ToList();
        }

        private static string GetColumnString(DbColumnInfo it)
        {
            return $"{it.DbColumnName}  {it.DataType}  {it.Length} {it.Scale}   default:{it.DefaultValue} description:{it.ColumnDescription} pk:{it.IsPrimarykey} nullable:{it.IsNullable} identity:{it.IsIdentity} ";
        }

        private static string GetUpdateColumnString(DbColumnInfo it,DbColumnInfo old)
        {
            var result= $"{it.DbColumnName}  changes: ";
            if (it.DataType != old.DataType) 
            {
                result += $"  [DataType:{old.DataType}->{it.DataType}] ";
            }
            if (it.Length != old.Length)
            {
                result += $"  [Length:{old.Length}->{it.Length}] ";
            }
            if (it.Scale != old.Scale)
            {
                result += $"  [Scale:{old.Scale}->{it.Scale}] ";
            }
            if (it.ColumnDescription != old.ColumnDescription)
            {
                result += $"  [Description:{old.ColumnDescription}->{it.ColumnDescription}] ";
            }
            if (it.IsPrimarykey != old.IsPrimarykey)
            {
                result += $"  [Pk:{old.IsPrimarykey}->{it.IsPrimarykey}] ";
            }
            if (it.IsNullable != old.IsNullable)
            {
                result += $"  [Nullable:{old.IsNullable}->{it.IsNullable}] ";
            }
            if (it.IsIdentity != old.IsIdentity)
            {
                result += $"  [Identity:{old.IsIdentity}->{it.IsIdentity}] ";
            }
            return result;
        }
    }
    public class TableDifferenceInfo
    {
        public List<DiffColumsInfo> DeleteColums { get; set; } = new List<DiffColumsInfo>();
        public List<DiffColumsInfo> UpdateColums { get; set; } = new List<DiffColumsInfo>();
        public List<DiffColumsInfo> AddColums { get; set; } = new List<DiffColumsInfo>();
        public List<DiffColumsInfo> UpdateRemark { get; set; } = new List<DiffColumsInfo>();
        public bool IsDiff
        {
            get
            {
                return
                    (DeleteColums.Count>0 ||
                     UpdateColums.Count > 0 ||
                     AddColums.Count > 0 ||
                     UpdateRemark.Count > 0) ;
            }
        }

        public string TableName { get;  set; }
    }

    public class DiffColumsInfo
    {
        public string SqlTemplate { get; set; }
        public string Message { get; set; }
    }

    public class DiffTableInfo
    {
        public DbTableInfo OldTableInfo { get; set; }
        public DbTableInfo NewTableInfo { get; set; }
        public List<DbColumnInfo> OldColumnInfos { get; set; }
        public List<DbColumnInfo> NewColumnInfos { get; set; }
    }
}
