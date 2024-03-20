using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace SqlSugar 
{
    /// <summary>
    /// QuestDb RestAPI
    /// </summary>
    public class QuestDbRestAPI
    {
        internal string url = string.Empty;
        internal string authorization = string.Empty;
        internal static Random random = new Random();
        ISqlSugarClient db;
        public QuestDbRestAPI(ISqlSugarClient db)
        {

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = db.CurrentConnectionConfig.ConnectionString;
            this.db = db;
            string host = String.Empty;
            string username = String.Empty;
            string password = String.Empty;
            QuestDbRestAPHelper.SetRestApiInfo(builder, ref host, ref username, ref password);
            BindHost(host, username, password);
        }
        /// <summary>
        /// 执行SQL异步
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<string> ExecuteCommandAsync(string sql)
        {
            //HTTP GET 执行SQL
            var result = string.Empty;
            var client = new HttpClient();
            var url = $"{this.url}/exec?query={HttpUtility.UrlEncode(sql)}";
            if (!string.IsNullOrWhiteSpace(authorization))
                client.DefaultRequestHeaders.Add("Authorization", authorization);
            var httpResponseMessage = await client.GetAsync(url);
            result = await httpResponseMessage.Content.ReadAsStringAsync();
            return result;
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string ExecuteCommand(string sql)
        {
            return ExecuteCommandAsync(sql).GetAwaiter().GetResult();
        }

        public async Task<int> BulkCopyAsync<T>(T insertData) where T:class,new()
        {
            if (db.CurrentConnectionConfig.MoreSettings == null)
                db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings();
            db.CurrentConnectionConfig.MoreSettings.DisableNvarchar = true;
            var  sql= db.Insertable(insertData).ToSqlString();
            return (await ExecuteCommandAsync(sql)).Contains("OK")?1:0;
        }

        public  int BulkCopy<T>(T insertData) where T : class, new()
        {
            return BulkCopyAsync(insertData).GetAwaiter().GetResult();
        }


        /// <summary>
        /// 批量快速插入异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="that"></param>
        /// <param name="dateFormat">导入时，时间格式 默认:yyyy/M/d H:mm:ss</param>
        /// <returns></returns>
        public async Task<int>  BulkCopyAsync<T>(List<T> insertList,  string dateFormat = "yyyy/M/d H:mm:ss") where T : class
        {
            
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("BulkCopy功能需要启用RestAPI，程序启动时执行：RestAPIExtension.UseQuestDbRestAPI(\"localhost:9000\", \"username\", \"password\")");
            }
            var result = 0;
            var fileName = $"{Guid.NewGuid()}.csv";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            try
            {
                var client = new HttpClient();
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                var list = new List<Hashtable>();
                var name = db.EntityMaintenance.GetEntityInfo<T>().DbTableName;

                var key = "QuestDbBulkCopy" + typeof(T).FullName + typeof(T).GetHashCode();
                var columns = new ReflectionInoCacheService().GetOrCreate(key, () =>
                 db.CopyNew().DbMaintenance.GetColumnInfosByTableName(name));
                columns.ForEach(d =>
                {
                    if (d.DataType == "TIMESTAMP")
                    {
                        list.Add(new Hashtable()
                    {
                        { "name", d.DbColumnName },
                        { "type", d.DataType },
                        { "pattern", dateFormat}
                    });
                    }
                    else
                    {
                        list.Add(new Hashtable()
                    {
                        { "name", d.DbColumnName },
                        { "type", d.DataType }
                    });
                    }
                });
                var schema = JsonConvert.SerializeObject(list);
                //写入CSV文件
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    await csv.WriteRecordsAsync(insertList);
                }

                var httpContent = new MultipartFormDataContent(boundary);
                if (!string.IsNullOrWhiteSpace(this.authorization))
                    client.DefaultRequestHeaders.Add("Authorization", this.authorization);
                httpContent.Add(new StringContent(schema), "schema");
                httpContent.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "data");
                //boundary带双引号 可能导致服务器错误情况
                httpContent.Headers.Remove("Content-Type");
                httpContent.Headers.TryAddWithoutValidation("Content-Type",
                    "multipart/form-data; boundary=" + boundary);
                var httpResponseMessage =
                    await Post(client, name, httpContent);
                var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
                var splitByLine = QuestDbRestAPHelper.SplitByLine(readAsStringAsync);
                foreach (var s in splitByLine)
                {
                    if (s.Contains("Rows"))
                    {
                        var strings = s.Split('|');
                        if (strings[1].Trim() == "Rows imported")
                        {
                            result = Convert.ToInt32(strings[2].Trim());
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // ignored
                }
            }
            return result;
        }

        private Task<HttpResponseMessage> Post(HttpClient client, string name, MultipartFormDataContent httpContent)
        {
            try
            {
                return client.PostAsync($"{this.url}/imp?name={name}", httpContent);
            }
            catch (Exception)
            { 
                throw;
            }
        }

        /// <summary>
        /// 批量快速插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="that"></param>
        /// <param name="dateFormat">导入时，时间格式 默认:yyyy/M/d H:mm:ss</param>
        /// <returns></returns>
        public  int BulkCopy<T>(List<T> insertList, string dateFormat = "yyyy/M/d H:mm:ss") where T : class
        {
             return BulkCopyAsync(insertList, dateFormat).GetAwaiter().GetResult();
        } 
        private void BindHost(string host, string username, string password)
        {
            url = host + ":9000";
            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1);

            if (!url.ToLower().StartsWith("http"))
                url = $"http://{url}";
            //生成TOKEN
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                authorization = $"Basic {base64}";
            }
        }
    }
}
