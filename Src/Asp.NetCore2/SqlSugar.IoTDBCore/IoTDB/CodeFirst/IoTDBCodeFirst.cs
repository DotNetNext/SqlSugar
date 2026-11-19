using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBCodeFirst : CodeFirstProvider
    {
        public override void InitTables(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            this.Context.InitMappingInfo(entityType);
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(entityType);
            var options = IoTDBConnectionOptions.Parse(this.Context.CurrentConnectionConfig.ConnectionString);
            this.Context.DbMaintenance.CreateDatabase(options.Database);
            Execute(entityType, entityInfo);
        }

        protected override string GetTableName(EntityInfo entityInfo)
        {
            var name = base.GetTableName(entityInfo);
            return IoTDBConnectionOptions.Parse(this.Context.CurrentConnectionConfig.ConnectionString).GetDevicePath(name);
        }

        protected override void Execute(Type entityType, EntityInfo entityInfo)
        {
            var devicePath = GetTableName(entityInfo);
            entityInfo.DbTableName = devicePath;
            this.Context.MappingTables.Add(entityInfo.EntityName, devicePath);
            foreach (var column in entityInfo.Columns) column.DbTableName = devicePath;

            if (this.Context.DbMaintenance.IsAnyTable(devicePath, false))
                ExistLogic(entityInfo);
            else
                NoExistLogic(entityInfo);
        }

        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var path = GetTableName(entityInfo);
            var columns = entityInfo.Columns.Where(it => !it.IsIgnore).Select(it => EntityColumnToDbColumn(entityInfo, path, it)).ToList();
            this.Context.DbMaintenance.CreateTable(path, columns, false);
        }

        public override void ExistLogic(EntityInfo entityInfo)
        {
            var path = GetTableName(entityInfo);
            var existing = this.Context.DbMaintenance.GetColumnInfosByTableName(path, false);
            foreach (var column in entityInfo.Columns.Where(it => !it.IsIgnore))
            {
                if (existing.Any(it => it.DbColumnName.Equals(column.DbColumnName, StringComparison.OrdinalIgnoreCase))) continue;
                this.Context.DbMaintenance.AddColumn(path, EntityColumnToDbColumn(entityInfo, path, column));
            }
        }
    }
}
