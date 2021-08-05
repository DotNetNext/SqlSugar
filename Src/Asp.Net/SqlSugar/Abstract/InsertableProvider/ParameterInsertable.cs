using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ParameterInsertable<T> : IParameterInsertable<T> where T:class,new()
    {
        internal IInsertable<T>   Inserable { get; set; }
        internal SqlSugarProvider Context { get; set; }
        public int ExecuteCommand() 
        {
            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns= inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it=>it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            this.Context.Utilities.PageEach(objects, 60, pagelist =>
            {
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("begin");
                foreach (var item in pagelist)
                {
                    var itemable = this.Context.Insertable(item);
                    itemable.InsertBuilder.DbColumnInfoList = itemable.InsertBuilder.DbColumnInfoList.Where(it => columns.Contains(it.DbColumnName)).ToList();
                    itemable.InsertBuilder.TableWithString = tableWithString;
                    (itemable as InsertableProvider<T>).RemoveCacheFunc = removeCacheFunc;
                    itemable.AddQueue();
                }
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("end \r\n");
                result +=this.Context.SaveQueues(false);
            });
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                return objects.Length;
            return result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns = inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it => it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            await this.Context.Utilities.PageEachAsync<T,int>(objects, 60,async pagelist =>
            {
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("Begin");
                foreach (var item in pagelist)
                {
                    var itemable = this.Context.Insertable(item);
                    itemable.InsertBuilder.DbColumnInfoList = itemable.InsertBuilder.DbColumnInfoList.Where(it => columns.Contains(it.DbColumnName)).ToList();
                    itemable.InsertBuilder.TableWithString = tableWithString;
                    (itemable as InsertableProvider<T>).RemoveCacheFunc = removeCacheFunc;
                    itemable.AddQueue();
                }
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle) 
                    this.Context.AddQueue("End");
                result += await this.Context.SaveQueuesAsync(false);
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    return objects.Length;
                return result;
            });
            return result;
        }
    }
}
