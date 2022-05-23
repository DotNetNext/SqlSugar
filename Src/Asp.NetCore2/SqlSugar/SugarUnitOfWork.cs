using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SugarUnitOfWork : IDisposable
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
            if (IsClose == false)
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

        public RepositoryType GetMyRepository<RepositoryType>() where RepositoryType : ISugarRepository, new()
        {
            var result = new RepositoryType();
            var type = typeof(RepositoryType).GetGenericArguments()[0];
            TenantAttribute tenantAttribute = type.GetCustomAttribute<TenantAttribute>();
            if (tenantAttribute == null)
            {
                result.Context = this.Db;
            }
            else 
            {
                result.Context = this.Db.AsTenant().GetConnection(tenantAttribute.configId);
            }
            return result;
        }

        public void Commit()
        {
            if (this.IsTran && this.IsCommit == false)
            {
                this.Tenant.CommitTran();
                IsCommit = true;
            }
            if (this.IsClose == false)
            {
                this.Db.Close();
                IsClose = true;
            }
        }
    }
}
