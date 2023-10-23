using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System;

namespace SqlSugar
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SugarTable : Attribute {
        public SugarTable() { }
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public bool IsDisabledDelete { get; set; }
        public bool IsDisabledUpdateAll { get; set; }
        public bool IsCreateTableFiledSort { get; set; }
        public string Discrimator { get; set; } 
        public SugarTable(string tableName) {
            this.TableName = tableName;
        }
        public SugarTable(string tableName,string tableDescription)
        {
            this.TableName = tableName;
            this.TableDescription = tableDescription;
        }

        public SugarTable(string tableName, string tableDescription,bool isDisabledDelete)
        {
            this.TableName = tableName;
            this.TableDescription = tableDescription;
            this.IsDisabledDelete = isDisabledDelete;
        }
        public SugarTable(string tableName, string tableDescription, bool isDisabledDelete, bool isCreateTableFieldSort)
        {
            this.TableName = tableName;
            this.TableDescription = tableDescription;
            this.IsDisabledDelete = isDisabledDelete;
            this.IsCreateTableFiledSort = isCreateTableFieldSort;
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

        public string[] UniqueGroupNameList { get; set; }

        private bool _IsArray;
        public bool IsArray
        {
            get { return _IsArray; }
            set { _IsArray = value; }
        }

        private bool _IsTreeKey;
        public bool IsTreeKey
        {
            get { return _IsTreeKey; }
            set { _IsTreeKey = value; }
        }

        public object SqlParameterDbType { get; set; }
        public object SqlParameterSize { get; set; }
        public int CreateTableFieldSort { get; set; }
        public bool InsertServerTime { get; set; }
        public string InsertSql { get; set; }
        public bool UpdateServerTime { get; set; }
        public string UpdateSql { get; set; }
        public object ExtendedAttribute{ get; set; }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TenantAttribute :Attribute
    {
        public object configId { get; set; }
        public TenantAttribute(object configId) 
        {
            this.configId = configId;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class Navigate: Attribute
    {
        internal string Name { get; set; }
        internal string Name2 { get; set; }
        internal Type MappingType { get; set; }
        internal string MappingAId { get; set; }
        internal string MappingBId { get; set; }
        internal NavigateType NavigatType { get; set; }
        internal string WhereSql { get; set; }

        
        public string GetName()
        {
            return Name;
        }
        public string GetName2()
        {
            return Name2;
        }
        public Type GetMappingType()
        {
            return MappingType;
        }
        public string GetMappingAId()
        {
            return MappingAId;
        }
        public string GetMappingBId()
        {
            return MappingBId;
        }
        public NavigateType GetNavigateType()
        {
            return NavigatType;
        }

        public string GetWhereSql()
        {
            return WhereSql;
        }
        public Navigate(NavigateType navigatType,string IfSingleMasterTableColumn_IfListChildTableColumn)
        {
            this.Name = IfSingleMasterTableColumn_IfListChildTableColumn;
            this.NavigatType = navigatType;
        }
        public Navigate(NavigateType navigatType, string IfSingleMasterTableColumn_IfListChildTableColumn, string IfSingleChildTableColumn_IfListMasterTableColumn)
        {
            this.Name = IfSingleMasterTableColumn_IfListChildTableColumn;
            this.Name2 = IfSingleChildTableColumn_IfListMasterTableColumn;
            this.NavigatType = navigatType;
        }

        public Navigate(NavigateType navigatType, string IfSingleMasterTableColumn_IfListChildTableColumn, string IfSingleChildTableColumn_IfListMasterTableColumn, string whereSql)
        {
            this.Name = IfSingleMasterTableColumn_IfListChildTableColumn;
            this.Name2 = IfSingleChildTableColumn_IfListMasterTableColumn;
            this.NavigatType = navigatType;
            this.WhereSql = whereSql;
            //Check.ExceptionEasy(navigatType != NavigateType.OneToOne, "Currently, only one-to-one navigation configuration Sql conditions are supported", "目前导航配置Sql条件只支持一对一");
        }

        public Navigate(Type MappingTableType,string typeAiD,string typeBId)
        {
            this.MappingType = MappingTableType;
            this.MappingAId = typeAiD;
            this.MappingBId = typeBId;
            this.NavigatType = NavigateType.ManyToMany;
        }
        public Navigate(Type MappingTableType, string typeAiD, string typeBId,string mappingSql)
        {
            this.MappingType = MappingTableType;
            this.MappingAId = typeAiD;
            this.MappingBId = typeBId;
            this.NavigatType = NavigateType.ManyToMany;
            this.WhereSql+= mappingSql;
        }
    }



    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SugarIndexAttribute : Attribute
    {
        public string IndexName { get; set; }
        public Dictionary<string, OrderByType> IndexFields { get; set; }
        public bool IsUnique { get; set; }
        public SugarIndexAttribute(string indexName,string fieldName,OrderByType sortType,bool isUnique=false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName, sortType);
            this.IsUnique = isUnique;

        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            this.IsUnique = isUnique;
        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, string fieldName3, OrderByType sortType3, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            IndexFields.Add(fieldName3, sortType3);
            this.IsUnique = isUnique;
        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, string fieldName3, OrderByType sortType3, string fieldName4, OrderByType sortType4, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            IndexFields.Add(fieldName3, sortType3);
            IndexFields.Add(fieldName4, sortType4);
            this.IsUnique = isUnique;
        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, string fieldName3, OrderByType sortType3, string fieldName4, OrderByType sortType4,string fieldName5, OrderByType sortType5, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            IndexFields.Add(fieldName3, sortType3);
            IndexFields.Add(fieldName4, sortType4);
            IndexFields.Add(fieldName5, sortType5);
            this.IsUnique = isUnique;
        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, string fieldName3, OrderByType sortType3, string fieldName4, OrderByType sortType4, string fieldName5, OrderByType sortType5, string fieldName6, OrderByType sortType6, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            IndexFields.Add(fieldName3, sortType3);
            IndexFields.Add(fieldName4, sortType4);
            IndexFields.Add(fieldName5, sortType5);
            IndexFields.Add(fieldName6, sortType6);
            this.IsUnique = isUnique;
        }
        public SugarIndexAttribute(string indexName, string fieldName1, OrderByType sortType1, string fieldName2, OrderByType sortType2, string fieldName3, OrderByType sortType3, string fieldName4, OrderByType sortType4, string fieldName5, OrderByType sortType5, string fieldName6, OrderByType sortType6, string fieldName7, OrderByType sortType7, bool isUnique = false)
        {
            this.IndexName = indexName;
            IndexFields = new Dictionary<string, OrderByType>();
            IndexFields.Add(fieldName1, sortType1);
            IndexFields.Add(fieldName2, sortType2);
            IndexFields.Add(fieldName3, sortType3);
            IndexFields.Add(fieldName4, sortType4);
            IndexFields.Add(fieldName5, sortType5);
            IndexFields.Add(fieldName6, sortType6);
            IndexFields.Add(fieldName7, sortType7);
            this.IsUnique = isUnique;
        }
    }

}
