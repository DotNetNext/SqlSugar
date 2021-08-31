using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// 代表一个SqlSugar的事务对象
    /// </summary>
    public interface ISugarTransaction: IDisposable
    {
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

    }
}
