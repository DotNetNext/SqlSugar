﻿using Pomelo.Data.MyCat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MyCatProvider : AdoProvider
    {
        public MyCatProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var mycatConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        if (!mycatConnectionString.ToLower().Contains("charset"))
                        {
                            mycatConnectionString = mycatConnectionString.Trim().TrimEnd(';') + ";charset=utf8;";
                        }
                        base._DbConnection = new MyCatConnection(mycatConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
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
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            base.BeginTran(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new MyCatDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            MyCatCommand sqlCommand = new MyCatCommand(sql, (MyCatConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (MyCatTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((MyCatParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((MyCatDataAdapter)dataAdapter).SelectCommand = (MyCatCommand)command;
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
            MyCatParameter[] result = new MyCatParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new MyCatParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                if (parameter.Direction == 0)
                {
                    parameter.Direction = ParameterDirection.Input; 
                }
                sqlParameter.Direction = parameter.Direction;
                //if (sqlParameter.Direction == 0)
                //{
                //    sqlParameter.Direction = ParameterDirection.Input;
                //}
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
                ++index;
            }
            return result;
        }
    }
}
