using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        public SqlBuilderProvider()
        {
        }

        public CommandType CommandType { get; set; }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public abstract string GetTranslationTableName(string name);
        public abstract string GetTranslationColumnName(string name);

        public string AppendWhereOrAnd(bool isWhere, string sqlString)
        {
            return isWhere ? (" WHERE " + sqlString ):( " AND " + sqlString);
        }

        public DeleteBuilder DeleteBuilder
        {
            get
            {
                base._DeleteBuilder = PubMethod.IsNullReturnNew(base._DeleteBuilder);
             //   base._DeleteBuilder.Conext = this.Context;
                return base._DeleteBuilder;
            }
            set { base._DeleteBuilder = value; }
        }

        public InsertBuilder InsertBuilder
        {
            get
            {
                base._InsertBuilder = PubMethod.IsNullReturnNew(base._InsertBuilder);
            //    base._InsertBuilder.Conext = this.Context;
                return base._InsertBuilder;
            }
            set { base._InsertBuilder = value; }
        }

        public LambadaQueryBuilder LambadaQueryBuilder
        {
            get;set;
        }

        public SqlQueryBuilder SqlQueryBuilder
        {
            get
            {
                base._SqlQueryBuilder = PubMethod.IsNullReturnNew(base._SqlQueryBuilder);
              //  base._SqlQueryBuilder.Conext = this.Context;
                return base._SqlQueryBuilder;
            }
            set { base._SqlQueryBuilder = value; }
        }

        public UpdateBuilder UpdateBuilder
        {
            get
            {
                base._UpdateBuilder = PubMethod.IsNullReturnNew(base._UpdateBuilder);
             //   base._UpdateBuilder.Conext = this.Context;
                return base._UpdateBuilder;
            }
            set { base._UpdateBuilder = value; }
        }

        public SqlSugarClient Context
        {
            get;set;
        }
    }
}
