using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SqlSugar.TDengineAdo
{
    public class TDengineParameter:DbParameter
    {
        // Private fields to store the parameter properties
        public object value { get; set; }
        public string parameterName;
        private int size;
        private System.Data.DbType dbType;
        public bool IsMicrosecond { get; set; }
        public bool IsNanosecond { get; set; }

        // Constructor to initialize the parameter
        public TDengineParameter(string parameterName,object value,System.Data.DbType dbType= System.Data.DbType.Object, int size=0)
        {
            this.parameterName = parameterName;
            this.value = value;
            this.size = size;
            this.dbType = dbType;
        }

        // Implementing abstract properties and methods
        public override System.Data.DbType DbType
        {
            get { return this.dbType; }
            set { this.dbType = value; }
        }

        public override int Size
        {
            get { return size; }
            set { size = value; }
        }

        public override string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        public override object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        // Other properties and methods can be implemented as needed

        // The following methods are abstract, so they must be implemented

        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }

        public override string SourceColumn
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override bool IsNullable
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override DataRowVersion SourceVersion
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        } 
        public override ParameterDirection Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool SourceColumnNullMapping { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
} 