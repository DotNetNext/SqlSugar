using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace SqlSugar
{

    /// <summary>
    /// 云计算连接配置
    /// </summary>
    public class CloudConnectionConfig
    {
        /// <summary>
        /// 处理机率,值越大机率越高
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 链接字符串名称
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// 云搜索Task反回类
    /// </summary>
    public class CloudSearchResult<T>
    {
        /// <summary>
        /// 集合
        /// </summary>
        public List<T> Entities { get; set; }
        /// <summary>
        /// 单个对象
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// DataTable
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }
  
    /// <summary>
    /// 云计扩展类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Taskable<T>
    {
        /// <summary>
        /// 任务
        /// </summary>
        public Task<CloudSearchResult<T>>[] Tasks { get; set; }
        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 数据库参数(例如:new{id=1,name="张三"})
        /// </summary>
        public object WhereObj { get; set; }

    }

    /// <summary>
    /// 云计扩展类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskableWithCount<T>
    {
        /// <summary>
        /// 任务
        /// </summary>
        public Task<CloudSearchResult<T>>[] Tasks { get; set; }
        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 数据库参数(例如:new{id=1,name="张三"})
        /// </summary>
        public object WhereObj { get; set; }

    }
}
