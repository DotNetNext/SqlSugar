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
        public string TableDescription { get; set; }
        public SugarTable(string tableName) {
            this.TableName = tableName;
        }
        public SugarTable(string tableName,string tableDescription)
        {
            this.TableName = tableName;
            this.TableDescription = tableDescription;
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

        private bool _IsOnlyIgnoreUpdate;
        public bool IsOnlyIgnoreUpdate
        {
            get { return _IsOnlyIgnoreUpdate; }
            set { _IsOnlyIgnoreUpdate = value; }
        }


        private bool _IsEnableUpdateVersionValidation;
        public bool IsEnableUpdateVersionValidation {
            get { return _IsEnableUpdateVersionValidation; }
            set { _IsEnableUpdateVersionValidation = value; }
        }



        private bool _IsTranscoding;
        public bool IsTranscoding
        {
            get { return _IsTranscoding; }
            set { _IsTranscoding = value; }
        }

        private bool _NoSerialize;
        public bool NoSerialize
        {
            get { return _NoSerialize; }
            set { _NoSerialize = value; }
        }

        private string _SerializeDateTimeFormat;
        public string SerializeDateTimeFormat
        {
            get { return _SerializeDateTimeFormat; }
            set { _SerializeDateTimeFormat = value; }
        }

        private bool _IsJson;
        public bool IsJson
        {
            get { return _IsJson; }
            set { _IsJson = value; }
        }


        private string _DefaultValue;
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        private string[] _IndexGroupNameList;
        public string[] IndexGroupNameList
        {
            get { return _IndexGroupNameList; }
            set { _IndexGroupNameList = value; }
        }

        public string[] UIndexGroupNameList { get; set; }

        private bool _IsArray;
        public bool IsArray
        {
            get { return _IsArray; }
            set { _IsArray = value; }
        }

    }

}
