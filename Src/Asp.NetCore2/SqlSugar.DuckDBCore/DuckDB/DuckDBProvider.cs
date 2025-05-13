using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DuckDB;
using DuckDB.NET.Data;
using DuckDB.NET.Native;

namespace SqlSugar.DuckDB
{
    public partial class DuckDBProvider : AdoProvider
    {
        public DuckDBProvider() 
        {
            if (StaticConfig.AppContext_ConvertInfinityDateTime == false)
            { 
                AppContext.SetSwitch("DuckDB.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("DuckDB.DisableDateTimeInfinityConversions", true);
            }
         
        }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var DuckDBConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new DuckDBConnection(DuckDBConnectionString);
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
            return new DuckDBDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var helper = new DuckDBInsertBuilder();
            helper.Context = this.Context;
            List<SugarParameter> orderParameters = new List<SugarParameter>();
            if (parameters.HasValue())
            {
                foreach (var p in parameters)
                {
                    if (!p.ParameterName.StartsWith(this.SqlParameterKeyWord))
                    {
                        p.ParameterName = this.SqlParameterKeyWord + p.ParameterName;
                    }
                }
                orderParameters = parameters.Where(it => sql.Contains(it.ParameterName))
                                           .Select(it => new { p = it, sort = GetSortId(sql, it) })
                                           .OrderBy(it => it.sort)
                                           .Where(it => it.sort != 0)
                                           .Select(it => it.p)
                                           .ToList();
                foreach (var param in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    sql = sql.Replace(param.ParameterName, "?");
                }

            }
            DuckDBCommand sqlCommand = new DuckDBCommand(sql, (DuckDBConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (DuckDBTransaction)this.Transaction;
            }
            if (orderParameters.HasValue())
            {
                DuckDBParameter[] ipars = (DuckDBParameter[])ToIDbDataParameter(orderParameters.ToArray());
                sqlCommand.Parameters.AddRange(ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
   
        private static int GetSortId(string sql, SugarParameter it)
        {
            return new List<int>() {
                                                  0,
                                                  sql.IndexOf(it.ParameterName+")"),
                                                  sql.IndexOf(it.ParameterName+" "),
                                                  sql.IndexOf(it.ParameterName+"="),
                                                  sql.IndexOf(it.ParameterName+"+"),
                                                  sql.IndexOf(it.ParameterName+"-"),
                                                  sql.IndexOf(it.ParameterName+";"),
                                                  sql.IndexOf(it.ParameterName+","),
                                                  sql.IndexOf(it.ParameterName+"*"),
                                                  sql.IndexOf(it.ParameterName+"/"),
                                                  sql.IndexOf(it.ParameterName+"|"),
                                                  sql.IndexOf(it.ParameterName+"&"),
                                                  sql.EndsWith(it.ParameterName)?sql.IndexOf(it.ParameterName):0
                                           }.Max();
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((DuckDBDataAdapter)dataAdapter).SelectCommand = (DuckDBCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            DuckDBParameter[] result = new DuckDBParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if (parameter.Value is System.Data.SqlTypes.SqlDateTime && parameter.DbType == System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                UNumber(parameter);
                var sqlParameter = new DuckDBParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                //if (parameter.IsJson)
                //{
                //    sqlParameter.DuckDBDbType = DuckDBType.Json;
                //} 
                if (sqlParameter.Direction == 0)
                {
                    sqlParameter.Direction = ParameterDirection.Input;
                }
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                if (isVarchar && sqlParameter.DbType == System.Data.DbType.String)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiString;
                }
                else if (sqlParameter.Value is DateTime && sqlParameter.DbType == System.Data.DbType.AnsiString)
                {
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                ++index;
                //if (parameter.CustomDbType != null&& parameter.CustomDbType is DuckDBDbType) 
                //{
                //    sqlParameter.DbType = ((DuckDBType)parameter.CustomDbType);
                //}
                sqlParameter.ParameterName = null;
            } 
            return result;
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
    }
}
