using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SugarAsyncLock : IDisposable
    {
        static  readonly SemaphoreSlim SemaphoreSlim =new SemaphoreSlim(1);

 
        public SugarAsyncLock(SqlSugarProvider db) 
        {

        }
         
        public async Task<SugarAsyncLock> AsyncLock(int timeOutSeconds)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(timeOutSeconds);
            await SemaphoreSlim.WaitAsync(timeout);
            return this;
        }

        public void Dispose()
        {
            SemaphoreSlim.Release();
        }
    }
}
