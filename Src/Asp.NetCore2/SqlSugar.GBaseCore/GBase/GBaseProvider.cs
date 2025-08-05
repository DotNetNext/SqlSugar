using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using GBS.Data.GBasedbt;

namespace SqlSugar.GBase
{
    public class GBaseProvider : AdoProvider
    {
        public GBaseProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        base._DbConnection = new GbsConnection(base.Context.CurrentConnectionConfig.ConnectionString?.Replace("Driver={GBase ODBC DRIVER (64-Bit)};",string.Empty));
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

        public string SplitCommandTag => UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", "");

        public override Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>> SqlQuery<T, T2, T3, T4, T5, T6, T7>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMappingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            string sqlString = builder.SqlQueryBuilder.ToSqlString();
            SugarParameter[] Parameters = builder.SqlQueryBuilder.Parameters.ToArray();
            this.GetDataBefore(sqlString, Parameters);
            using (var dataReader = this.GetDataReader(sqlString, Parameters))
            {
                DbDataReader DbReader = (DbDataReader)dataReader;
                List<T> result = new List<T>();
                result = GetData<T>(typeof(T), dataReader);
          
                List<T2> result2 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T2>();
                    result2 = GetData<T2>(typeof(T2), dataReader);
                }
                List<T3> result3 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T3>();
                    result3 = GetData<T3>(typeof(T3), dataReader);
                }
                List<T4> result4 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T4>();
                    result4 = GetData<T4>(typeof(T4), dataReader);
                }
                List<T5> result5 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T5>();
                    result5 = GetData<T5>(typeof(T5), dataReader);
                }
                List<T6> result6 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T6>();
                    result6 = GetData<T6>(typeof(T6), dataReader);
                }
                List<T7> result7 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T7>();
                    result7 = GetData<T7>(typeof(T7), dataReader);
                }
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                this.GetDataAfter(sqlString, Parameters);
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>(result, result2, result3, result4, result5, result6, result7);
            }
        }
        public override async Task<Tuple<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> SqlQueryAsync<T, T2, T3, T4, T5, T6, T7>(string sql, object parameters = null)
        {
            var parsmeterArray = this.GetParameters(parameters);
            this.Context.InitMappingInfo<T>();
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parsmeterArray != null && parsmeterArray.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parsmeterArray);
            string sqlString = builder.SqlQueryBuilder.ToSqlString();
            SugarParameter[] Parameters = builder.SqlQueryBuilder.Parameters.ToArray();
            this.GetDataBefore(sqlString, Parameters);
            using (var dataReader = await this.GetDataReaderAsync(sqlString, Parameters))
            {
                DbDataReader DbReader = (DbDataReader)dataReader;
                List<T> result = new List<T>(); 
               result = await GetDataAsync<T>(typeof(T), dataReader); 
                List<T2> result2 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T2>();
                    result2 = await GetDataAsync<T2>(typeof(T2), dataReader);
                }
                List<T3> result3 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T3>();
                    result3 = await GetDataAsync<T3>(typeof(T3), dataReader);
                }
                List<T4> result4 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T4>();
                    result4 = await GetDataAsync<T4>(typeof(T4), dataReader);
                }
                List<T5> result5 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T5>();
                    result5 = await GetDataAsync<T5>(typeof(T5), dataReader);
                }
                List<T6> result6 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T6>();
                    result6 = await GetDataAsync<T6>(typeof(T6), dataReader);
                }
                List<T7> result7 = null;
                if (NextResult(dataReader))
                {
                    this.Context.InitMappingInfo<T7>();
                    result7 = await GetDataAsync<T7>(typeof(T7), dataReader);
                }
                builder.SqlQueryBuilder.Clear();
                if (this.Context.Ado.DataReaderParameters != null)
                {
                    foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                    {
                        var parameter = parsmeterArray.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                        if (parameter != null)
                        {
                            parameter.Value = item.Value;
                        }
                    }
                    this.Context.Ado.DataReaderParameters = null;
                }
                this.GetDataAfter(sqlString, Parameters);
                return Tuple.Create<List<T>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>(result, result2, result3, result4, result5, result6, result7);
            }
        }

        public override object GetScalar(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null)
            {
                return _GetScalar(sql, parameters);
            }
            else 
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = _GetScalar(sql, parameters);
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    this.Context.Ado.RollbackTran();
                    throw ex;
                }
            }
        }

        public override async Task<object> GetScalarAsync(string sql, params SugarParameter[] parameters)
        {
            if (this.Context.Ado.Transaction != null)
            {
                return await _GetScalarAsync(sql, parameters);
            }
            else
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = await _GetScalarAsync(sql, parameters);
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    this.Context.Ado.RollbackTran();
                    throw ex;
                }
            }
        }
        private object _GetScalar(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result = base.GetScalar(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return base.GetScalar(sql, parameters);
            }
        }
        private async Task<object> _GetScalarAsync(string sql, SugarParameter[] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                object result = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        result = await base.GetScalarAsync(item, parameters);
                    }
                }
                return result;
            }
            else
            {
                return await base.GetScalarAsync(sql, parameters);
            }
        }

        public override int ExecuteCommand(string sql, SugarParameter [] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                int i = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        // parameter name which has prefix is add by AddBatchInsertParameters().
                        // it should be passed to Command row by row. 
                        // the parameter which has same prefix is in the same row.
                        var namePrefix = i.ToString() + this.Context.Ado.SqlParameterKeyWord;
                        var paramParts = parameters.Where(o => string.Compare(o.ParameterName, 0, namePrefix, 0, namePrefix.Length) == 0).ToArray();

                        result += base.ExecuteCommand(item, paramParts.Length > 0 ? paramParts : parameters);
                    }
                    ++i;
                }
                return result;
            }
            else
            {
                return base.ExecuteCommand(sql, parameters);
            }
        }
        public override async Task<int> ExecuteCommandAsync(string sql, SugarParameter [] parameters)
        {
            if (sql == null) throw new Exception("sql is null");
            if (sql.IndexOf(this.SplitCommandTag) > 0)
            {
                var sqlParts = Regex.Split(sql, this.SplitCommandTag).Where(it => !string.IsNullOrEmpty(it)).ToList();
                int result = 0;
                int i = 0;
                foreach (var item in sqlParts)
                {
                    if (item.TrimStart('\r').TrimStart('\n') != "")
                    {
                        // parameter name which has prefix is add by AddBatchInsertParameters().
                        // it should be passed to Command row by row. 
                        // the parameter which has same prefix is in the same row.
                        var namePrefix = i.ToString() + this.Context.Ado.SqlParameterKeyWord;
                        var paramParts = parameters.Where(o => string.Compare(o.ParameterName, 0, namePrefix, 0, namePrefix.Length) == 0).ToArray();

                        result += await base.ExecuteCommandAsync(item, paramParts.Length > 0 ? paramParts : parameters);
                    }
                    ++i;
                }
                return result;
            }
            else
            {
                return base.ExecuteCommand(sql, parameters);
            }
        }

        /// <summary>
        /// Only GBase
        /// </summary>
        /// <param name="transactionName"></param>
        public override void BeginTran(string transactionName)
        {
            CheckConnection();
            base.Transaction = ((GbsConnection)this.Connection).BeginTransaction();
        }
        /// <summary>
        /// Only GBase
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            CheckConnection();
            base.Transaction = ((GbsConnection)this.Connection).BeginTransaction(iso);
        }
     
        public override IDataAdapter GetAdapter()
        {
            return new GBaseDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var helper = new GBaseInsertBuilder();
            helper.Context = this.Context;
            GbsCommand sqlCommand = ((GbsConnection)this.Connection).CreateCommand();
            if (parameters != null)
            {
                foreach (var param in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    if (param.Direction == 0)
                        param.Direction = System.Data.ParameterDirection.Input;
                    if ((sql.Contains(param.ParameterName) && UtilMethods.HasBigObjectParam(param)) ||
                        this.CommandType == CommandType.StoredProcedure)
                    {
                        // for big object data, in the insert or update statements
                        // the charactor after the @ParameterName could only be , or ) or space.
                        // here use these characters as postfix of the parameter name.
                        // the Replace method would only replace one field each time.
                        sql = sql.Replace(param.ParameterName + ",", " ?, ");
                        sql = sql.Replace(param.ParameterName + ")", " ?) ");
                        sql = sql.Replace(param.ParameterName + " ", " ?  ");

                        var gbsParam = sqlCommand.CreateParameter();
                        gbsParam.DbType = param.DbType;
                        gbsParam.ParameterName = param.ParameterName;
                        gbsParam.Direction = param.Direction;

                        if (UtilMethods.HasBigObjectParam(param))
                        {
                            // assign GbsType.
                            switch (param.TypeName)
                            {
                                case "blob":
                                    gbsParam.GbsType = GbsType.Blob;
                                    gbsParam.Value = (param.Value == null) ? string.Empty : param.Value;
                                    break;
                                case "clob":
                                    gbsParam.GbsType = GbsType.Clob;
                                    gbsParam.Value = (param.Value == null) ? string.Empty : param.Value;
                                    break;
                                case "text":
                                    gbsParam.GbsType = GbsType.Text;
                                    gbsParam.Value = (param.Value == null) ? DBNull.Value : param.Value;
                                    break;
                                case "byte":
                                default:
                                    gbsParam.GbsType = GbsType.Byte;
                                    gbsParam.Value = (param.Value == null) ? DBNull.Value : param.Value;
                                    break;
                            }
                        }
                        else
                        {
                            gbsParam.Value = (param.Value == null) ? DBNull.Value : param.Value;
                        }

                        sqlCommand.Parameters.Add(gbsParam);
                        if (gbsParam.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                        {
                            if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                            this.OutputParameters.RemoveAll(it => it.ParameterName == gbsParam.ParameterName);
                            this.OutputParameters.Add(gbsParam);
                        }
                    }
                    else
                    {
                        sql = sql.Replace(param.ParameterName, helper.FormatValue(param.Value) + "");
                    }
                }
            }
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (sqlCommand?.Parameters?.Count > 0)
            {
                if (this.CommandType == CommandType.StoredProcedure && parameters != null && sqlCommand.Parameters.Count == parameters.Length)
                {
                    // 保证存储过程参数顺序与 SugarParameter 一致
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var sugarParam = parameters[i];
                        var dbParam = sqlCommand.Parameters.Cast<DbParameter>().FirstOrDefault(p => p.ParameterName == sugarParam.ParameterName);
                        if (dbParam != null && sqlCommand.Parameters.IndexOf(dbParam) != i)
                        {
                            sqlCommand.Parameters.Remove(dbParam);
                            sqlCommand.Parameters.Insert(i, dbParam);
                        }
                    }
                }
            } 
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (GbsTransaction)this.Transaction;
            }
            //if (parameters.HasValue())
            //{
            //    OdbcParameter[] ipars = GetSqlParameter(parameters);
            //    sqlCommand.Parameters.AddRange(ipars);
            //}
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((GBaseDataAdapter)dataAdapter).SelectCommand = (GbsCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return new GbsParameter[] { };
            GbsParameter[] result = new GbsParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new GbsParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                //sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                if (parameter.Direction == 0) 
                {
                    parameter.Direction = ParameterDirection.Input;
                }
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput,ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                ++index;
            }
            return result;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public GbsParameter[] GetSqlParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            GbsParameter[] result = new GbsParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new GbsParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                //sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = GetDbType(parameter);
                var isTime = parameter.DbType == System.Data.DbType.Time;
                if (isTime)
                {
                    sqlParameter.Value = DateTime.Parse(parameter.Value?.ToString()).TimeOfDay;
                }
                if (sqlParameter.Value != null && sqlParameter.Value != DBNull.Value && sqlParameter.DbType == System.Data.DbType.DateTime)
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date == DateTime.MinValue)
                    {
                        sqlParameter.Value = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                }
                if (parameter.Direction == 0)
                {
                    parameter.Direction = ParameterDirection.Input;
                }
                sqlParameter.Direction = parameter.Direction;
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

                ++index;
            }
            return result;
        }

        private static System.Data.DbType GetDbType(SugarParameter parameter)
        {
            if (parameter.DbType==System.Data.DbType.UInt16)
            {
                return System.Data.DbType.Int16;
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                return System.Data.DbType.Int32;
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                return System.Data.DbType.Int64;
            }
            else
            {
                return parameter.DbType;
            }
        }
    }
}
