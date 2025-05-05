using Microsoft.Data.SqlClient;
using MongoDb.Ado.data;
using SqlSugar.MongoDbCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions; 

namespace SqlSugar.MongoDb
{
    public partial class MongoDbProvider : AdoProvider
    {
        public MongoDbProvider() 
        {
            if (StaticConfig.AppContext_ConvertInfinityDateTime == false)
            { 
                AppContext.SetSwitch("MongoDb.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("MongoDb.DisableDateTimeInfinityConversions", true);
            }
         
        }
        public override bool IsNoSql { get; set; } = true;
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var MongoDbConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new MongoDbConnection(MongoDbConnectionString);
                    }
                    catch (Exception)
                    {
                        throw;

                    }
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }

        public override void BeginTran(string transactionName)
        {
            base.BeginTran();
        }
       
        public override IDataAdapter GetAdapter()
        {
            return new SqlSugarMongoDbDataAdapter();
        } 
        
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        { 
            return parameters;
        }
 
        private static void UNumber(SugarParameter parameter)
        {
            if (parameter.DbType == System.Data.DbType.UInt16)
            {
                parameter.DbType = System.Data.DbType.Int16;
                parameter.Value = Convert.ToInt16(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                parameter.DbType = System.Data.DbType.Int32;
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                parameter.DbType = System.Data.DbType.Int64;
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
        }

        public override void BeginTran(System.Data.IsolationLevel iso, string transactionName)
        {
            base.BeginTran();
        }

        public override void SetCommandToAdapter(IDataAdapter adapter, DbCommand command)
        {
            ((MongoDbDataAdapter)adapter).SelectCommand =(MongoDbCommand)command;
        }

        public override DbCommand GetCommand(string sql, SugarParameter[] pars)
        {
            MongoDbCommand mongoDbCommand = new MongoDbCommand(sql,(MongoDbConnection)this.Connection);
            CheckConnection();
            return mongoDbCommand;
        }
    }
}
