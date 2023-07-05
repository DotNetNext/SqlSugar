using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISugarUnitOfWork<T> where T : SugarUnitOfWork, new()
    {
        ISqlSugarClient Db { get; set; }
        T CreateContext(bool isTran=true);
    }
    public class SugarUnitOfWork<T> : ISugarUnitOfWork<T> where T : SugarUnitOfWork, new()
    {
        public SugarUnitOfWork(ISqlSugarClient db)
        {
            this.Db = db;
        }
        public ISqlSugarClient Db { get; set; }
        public T CreateContext(bool isTran=true)
        {
            return Db.CreateContext<T>(isTran);
        }
    }
    public interface ISugarUnitOfWorkClear
    {
        RepositoryType GetMyRepository<RepositoryType>() where RepositoryType : new();

        bool Commit();
    }
    /// <summary>
    /// ISugarUnitOfWorkClear  not exists SqlSugar method
    /// ISugarUnitOfWork exists SqlSugar method
    /// </summary>
    public interface ISugarUnitOfWork : ISugarUnitOfWorkClear
    {
        ISqlSugarClient Db { get;  }
        ITenant Tenant { get;  }

        SimpleClient<T> GetRepository<T>() where T : class, new();
    }
    /// <summary>
    /// SugarUnitOfWork->ISugarUnitOfWork->ISaugarUnitOfWorkClear
    /// ISaugarUnitOfWorkClear  not exists SqlSugar method
    /// ISugarUnitOfWork exists SqlSugar method
    /// </summary>
    public class SugarUnitOfWork : IDisposable, ISugarUnitOfWork
    {
        public ISqlSugarClient Db { get; internal set; }
        public ITenant Tenant { get; internal set; }
        public bool IsTran { get; internal set; }
        public bool IsCommit { get; internal set; }
        public bool IsClose { get; internal set; }

        public void Dispose()
        {

            if (this.IsTran && IsCommit == false)
            {
                this.Tenant.RollbackTran();
            }
            if (this.Db.Ado.Transaction==null&&IsClose == false)
            {
                this.Db.Close();
            }
        }

        public SimpleClient<T> GetRepository<T>() where T : class, new()
        {
            TenantAttribute tenantAttribute = typeof(T).GetCustomAttribute<TenantAttribute>();
            if (tenantAttribute == null)
            {
                return new SimpleClient<T>(Db);
            }
            else 
            {
                return new SimpleClient<T>(Db.AsTenant().GetConnection(tenantAttribute.configId));
            }
        }

        public RepositoryType GetMyRepository<RepositoryType>() where RepositoryType:new()
        {
            var result = (ISugarRepository)new RepositoryType();
            var type = typeof(RepositoryType).GetGenericArguments().FirstOrDefault();
            TenantAttribute tenantAttribute = type?.GetCustomAttribute<TenantAttribute>();
            if (tenantAttribute == null)
            {
                result.Context = this.Db;
            }
            else 
            {
                result.Context = this.Db.AsTenant().GetConnection(tenantAttribute.configId);
            }
            return (RepositoryType)result;
        }

        public bool Commit()
        {
            if (this.IsTran && this.IsCommit == false)
            {
                this.Tenant.CommitTran();
                IsCommit = true;
            }
            if (this.Db.Ado.Transaction==null&&this.IsClose == false)
            {
                this.Db.Close();
                IsClose = true;
            }
            return IsCommit;
        }
    }
}
