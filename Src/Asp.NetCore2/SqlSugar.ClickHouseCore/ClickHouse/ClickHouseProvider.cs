using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
 

namespace SqlSugar.ClickHouse
{
    public partial class ClickHouseProvider : AdoProvider
    {
        public ClickHouseProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var ClickHouseConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new ClickHouseConnection(ClickHouseConnectionString);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }
        public  string SplitCommandTag => UtilConstants.ReplaceCommaKey;
        public override int ExecuteCommand(string sql, params SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag)>0)
            {
                var sqlParts=Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                var result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result += base.ExecuteCommand(item, parameters);
                    }
                }
                return result;
            }
            else 
            {
                return base.ExecuteCommand(sql, parameters);
            }
        }
        public override async Task<int> ExecuteCommandAsync(string sql, params SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                var result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result +=await base.ExecuteCommandAsync(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return await base.ExecuteCommandAsync(sql, parameters);
            }
        }
        public override void BeginTran()
        {
            //No Support
        }
        public override void BeginTran(string transactionName)
        {
            //No Support
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            //No Support
        }
        public override void CommitTran()
        {
            //No Support
        }
        public override void RollbackTran()
        {
            //No Support
        }
        public override IDataAdapter GetAdapter()
        {
            return new ClickHouseDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var connection=(ClickHouseConnection)this.Connection;
            CheckConnection();
            ClickHouseCommand sqlCommand =connection.CreateCommand();
            var pars = ToIDbDataParameter(parameters);
            var arrayPars = parameters?.Where(it =>it.IsArray)?.Where(it=>it.ParameterName!=null)?.Select(it=>it.ParameterName);
            foreach (var param in pars.OrderByDescending(it=>it.ParameterName.Length)) 
            {
                var newName = param.ParameterName.TrimStart('@');
                object dbtype = param.DbType;
                if (dbtype.ObjToString() == System.Data.DbType.Decimal.ToString()) 
                {
                    dbtype = ClickHouseDbBind.MappingTypesConst.First(it => it.Value == CSharpDataType.@decimal).Key;
                }
                if (dbtype.ObjToString() == System.Data.DbType.Guid.ToString())
                {
                    dbtype = ClickHouseDbBind.MappingTypesConst.First(it => it.Value == CSharpDataType.Guid).Key;
                    if (param.Value == DBNull.Value)
                    {
                        sql = sql.Replace(param.ParameterName, "null");
                    }
                }
                if (dbtype.ObjToString() == System.Data.DbType.SByte.ToString())
                {
                    dbtype = ClickHouseDbBind.MappingTypesConst.First(it => it.Value == CSharpDataType.@sbyte).Key;
                }
                if (param.Value != null && param.Value != DBNull.Value && dbtype.ObjToString() == System.Data.DbType.Boolean.ToString())
                {
                    sql = sql.Replace(param.ParameterName, param.Value.ObjToBool() ? "1" : "0");
                }
                else if (dbtype.ObjToString() == System.Data.DbType.Boolean.ToString())
                {
                    sql = sql.Replace(param.ParameterName, "null");
                }
                else if (arrayPars != null && arrayPars.Contains(param.ParameterName))
                {
                    if (param.Value == null)
                    {
                        sql = sql.Replace(param.ParameterName, "null");
                    }
                    else
                    {
                        sql = sql.Replace(param.ParameterName,   this.Context.Utilities.SerializeObject(param.Value).Replace("\"","'"));
                    }
                }
                else if (dbtype.ObjToString() == "DateTime" && param.Value == DBNull.Value)
                {
                    sql = sql.Replace(param.ParameterName, "null");
                }
                else if (dbtype.ObjToString() == "UUID" && param.Value == DBNull.Value)
                {
                    sql = sql.Replace(param.ParameterName, "null");
                }
                else
                {
                    sql = sql.Replace(param.ParameterName, "{" + newName + ":" + dbtype + "}");
                    
                }
                param.ParameterName = newName;
            }
            sqlCommand.CommandText = sql;
            sqlCommand.Parameters.AddRange(pars);
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((ClickHouseDataAdapter)dataAdapter).SelectCommand = (ClickHouseCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return new ClickHouseDbParameter[] { };
            ClickHouseDbParameter[] result = new ClickHouseDbParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if(parameter.Value is System.Data.SqlTypes.SqlDateTime&&parameter.DbType==System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                var sqlParameter = new ClickHouseDbParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                ++index;
                
            }
            return result;
        }
 
    }
}
