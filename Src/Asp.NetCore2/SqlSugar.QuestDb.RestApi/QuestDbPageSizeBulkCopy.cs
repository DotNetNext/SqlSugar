using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class QuestDbPageSizeBulkCopy
    {
        private QuestDbRestAPI questDbRestAPI;
        private int pageSize;
        private ISqlSugarClient db;
        public QuestDbPageSizeBulkCopy(QuestDbRestAPI questDbRestAPI, int pageSize, ISqlSugarClient db)
        {
            this.questDbRestAPI = questDbRestAPI;
            this.pageSize = pageSize;
            this.db = db;
        } 
        public int BulkCopy<T>(List<T> insertDatas, string dateFormat = "yyyy/M/d H:mm:ss") where T : class, new()
        {
            int result = 0;
            db.Utilities.PageEach(insertDatas, pageSize, pageItems => 
            {
                result+=questDbRestAPI.BulkCopyAsync(pageItems, dateFormat).GetAwaiter().GetResult();
            });
            return result;
        }
        public async Task<int> BulkCopyAsync<T>(List<T> insertDatas, string dateFormat = "yyyy/M/d H:mm:ss") where T : class, new()
        {
            int result = 0;
            await db.Utilities.PageEachAsync(insertDatas, pageSize, async pageItems =>
            {
                result +=await questDbRestAPI.BulkCopyAsync(pageItems, dateFormat);
            });
            return result;
        }
    }
}
