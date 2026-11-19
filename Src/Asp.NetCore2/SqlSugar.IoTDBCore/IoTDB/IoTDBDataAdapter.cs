using System;
using System.Data;
using System.Data.Common;

namespace SqlSugar.IoTDBCore
{
    public sealed class IoTDBDataAdapter : IDataAdapter
    {
        public DbCommand SelectCommand { get; set; }

        public MissingMappingAction MissingMappingAction { get; set; } = MissingMappingAction.Passthrough;
        public MissingSchemaAction MissingSchemaAction { get; set; } = MissingSchemaAction.Add;
        public ITableMappingCollection TableMappings { get; } = new DataTableMappingCollection();

        public void Fill(DataSet dataSet)
        {
            if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));
            var table = new DataTable();
            Fill(table);
            dataSet.Tables.Add(table);
        }

        public int Fill(DataTable dataTable)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
            if (SelectCommand == null) throw new InvalidOperationException("SelectCommand is required.");

            using (DbDataReader reader = SelectCommand.ExecuteReader())
            {
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    var name = reader.GetName(index);
                    if (dataTable.Columns.Contains(name)) name += index;
                    dataTable.Columns.Add(name, reader.GetFieldType(index) ?? typeof(object));
                }
                while (reader.Read())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    dataTable.Rows.Add(values);
                }
            }
            return dataTable.Rows.Count;
        }

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType) => throw new NotSupportedException("IoTDB does not expose ADO.NET schema fill for tree metadata.");
        public int Update(DataSet dataSet) => throw new NotSupportedException("IoTDB adapter is read-only; use SqlSugar Insertable for writes.");
    }
}
