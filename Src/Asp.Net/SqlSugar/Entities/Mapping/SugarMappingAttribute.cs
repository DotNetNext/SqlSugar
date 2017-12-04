using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System;

namespace SqlSugar
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SugarTable : Attribute {
        private SugarTable() { }
        public string TableName { get; set; }
        public SugarTable(string tableName) {
            this.TableName = tableName;
        }
    }
   [AttributeUsage(AttributeTargets.Property , Inherited = true)]
    public class SugarColumn : Attribute
    {
        private string _ColumnName;
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        private bool _IsIgnore;
        public bool IsIgnore
        {
            get { return _IsIgnore; }
            set { _IsIgnore = value; }
        }

        private bool _IsPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }

        private bool _IsIdentity;
        public bool IsIdentity
        {
            get { return _IsIdentity; }
            set { _IsIdentity = value; }
        }

        private string _MappingKeys;
        public string MappingKeys
        {
            get { return _MappingKeys; }
            set { _MappingKeys = value; }
        }

        private string _ColumnDescription;
        public string ColumnDescription
        {
            get { return _ColumnDescription; }
            set { _ColumnDescription = value; }
        }

        private int _Length;
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private bool _IsNullable;
        public bool IsNullable
        {
            get { return _IsNullable; }
            set { _IsNullable = value; }
        }

        private string _OldColumnName;
        public string OldColumnName
        {
            get { return _OldColumnName; }
            set { _OldColumnName = value; }
        }

        private string _ColumnDataType;
        public string ColumnDataType
        {
            get { return _ColumnDataType; }
            set { _ColumnDataType = value; }
        }

        private int  _DecimalDigits;
        public int  DecimalDigits {
            get { return _DecimalDigits; }
            set { _DecimalDigits = value; }
        }

        private string _OracleSequenceName;
        public string OracleSequenceName {
            get { return _OracleSequenceName; }
            set { _OracleSequenceName = value; }
        }

        private bool _IsOnlyIgnoreInsert;
        public bool IsOnlyIgnoreInsert
        {
            get { return _IsOnlyIgnoreInsert; }
            set { _IsOnlyIgnoreInsert = value; }
        }
    }

}
