using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class SugarParameter : DbParameter
    {
        public bool IsRefCursor { get; set; }
        public SugarParameter(string name, object value)
        {
            this.Value = value;
            this.ParameterName = name;
            if (value != null)
            {
                SettingDataType(value.GetType());
            }
        }
        public SugarParameter(string name, object value, Type type)
        {
            this.Value = value;
            this.ParameterName = name;
            SettingDataType(type);
        }
        public SugarParameter(string name, object value, Type type, ParameterDirection direction)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            SettingDataType(type);
        }
        public SugarParameter(string name, object value, Type type, ParameterDirection direction, int size)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.Size = size;
            SettingDataType(type);
        }


        public SugarParameter(string name, object value, System.Data.DbType type)
        {
            this.Value = value;
            this.ParameterName = name;
            this.DbType = type;
        }
        public SugarParameter(string name, DataTable value, string SqlServerTypeName)
        {
            this.Value = value;
            this.ParameterName = name;
            this.TypeName = SqlServerTypeName;
        }
        public SugarParameter(string name, object value, System.Data.DbType type, ParameterDirection direction)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.DbType = type;
        }
        public SugarParameter(string name, object value, System.Data.DbType type, ParameterDirection direction, int size)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.Size = size;
            this.DbType = type;
        }

        private void SettingDataType(Type type)
        {
            if (type == UtilConstants.ByteArrayType)
            {
                this.DbType = System.Data.DbType.Binary;
            }
            else if (type == UtilConstants.GuidType)
            {
                this.DbType = System.Data.DbType.Guid;
            }
            else if (type == UtilConstants.IntType)
            {
                this.DbType = System.Data.DbType.Int32;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.DbType = System.Data.DbType.Int16;
            }
            else if (type == UtilConstants.LongType)
            {
                this.DbType = System.Data.DbType.Int64;
            }
            else if (type == UtilConstants.DateType)
            {
                this.DbType = System.Data.DbType.DateTime;
            }
            else if (type == UtilConstants.DobType)
            {
                this.DbType = System.Data.DbType.Double;
            }
            else if (type == UtilConstants.DecType)
            {
                this.DbType = System.Data.DbType.Decimal;
            }
            else if (type == UtilConstants.ByteType)
            {
                this.DbType = System.Data.DbType.Byte;
            }
            else if (type == UtilConstants.FloatType)
            {
                this.DbType = System.Data.DbType.Single;
            }
            else if (type == UtilConstants.BoolType)
            {
                this.DbType = System.Data.DbType.Boolean;
            }
            else if (type == UtilConstants.StringType)
            {
                this.DbType = System.Data.DbType.String;
            }
            else if (type == UtilConstants.DateTimeOffsetType)
            {
                this.DbType = System.Data.DbType.DateTimeOffset;
            }
            else if (type == UtilConstants.TimeSpanType)
            {
                if (this.Value != null)
                    this.Value = this.Value.ToString();
            }
            else if (type.IsEnum())
            {
                this.DbType = System.Data.DbType.Int64;
            }

        }
        public SugarParameter(string name, object value, bool isOutput)
        {
            this.Value = value;
            this.ParameterName = name;
            if (isOutput)
            {
                this.Direction = ParameterDirection.Output;
            }
        }
        public override System.Data.DbType DbType
        {
            get; set;
        }

        public override ParameterDirection Direction
        {
            get; set;
        }

        public override bool IsNullable
        {
            get; set;
        }

        public override string ParameterName
        {
            get; set;
        }

        public int _Size;

        public override int Size
        {
            get
            {
                if (_Size == 0 && Value != null)
                {
                    var isByteArray = Value.GetType() == UtilConstants.ByteArrayType;
                    if (isByteArray)
                        _Size = -1;
                    else
                    {
                        var length = Value.ToString().Length;
                        _Size = length < 4000 ? 4000 : -1;

                    }
                }
                if (_Size == 0)
                    _Size = 4000;
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        public override string SourceColumn
        {
            get; set;
        }

        public override bool SourceColumnNullMapping
        {
            get; set;
        }
        public string UdtTypeName
        {
            get;
            set;
        }

        public override object Value
        {
            get; set;
        }

        public Dictionary<string, object> TempDate
        {
            get; set;
        }

        /// <summary>
        /// 如果类库是.NET 4.5请删除该属性
        /// If the SqlSugar library is.NET 4.5, delete the property
        /// </summary>
        public override DataRowVersion SourceVersion
        {
            get; set;
        }

        public override void ResetDbType()
        {
            this.DbType = System.Data.DbType.String;
        }


        public string TypeName { get; set; }
        public bool IsJson { get;  set; }
        public bool IsArray { get;  set; }
    }
}
