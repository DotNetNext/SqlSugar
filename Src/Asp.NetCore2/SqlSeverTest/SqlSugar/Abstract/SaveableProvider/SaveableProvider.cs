using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SaveableProvider<T> : ISaveable<T> where T : class, new()
    {
        internal SaveableProvider(SqlSugarProvider context,List<T> saveObjects)
        {
            this.saveObjects = saveObjects;
            this.Context = context;
            this.Context.InitMappingInfo<T>();
        }
        internal SaveableProvider(SqlSugarProvider context, T saveObject)
        {
            this.saveObjects = new List<T>() { saveObject };
            this.Context = context;
            this.Context.InitMappingInfo<T>();
        }
        public SqlSugarProvider Context { get; set; }
        public List<T> saveObjects = new List<T>();
        public List<T> existsObjects = null;
        public List<T> insertObjects
        {
            get
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                List<T> result = new List<T>();
                var pks = GetPrimaryKeys();
                Check.Exception(pks.IsNullOrEmpty(), "Need primary key");
                Check.Exception(pks.Count() > 1, "Multiple primary keys are not supported");
                var pkInfo = this.EntityInfo.Columns.Where(it=>it.IsIgnore==false).Where(it => it.DbColumnName.Equals(pks.First(), StringComparison.CurrentCultureIgnoreCase)).First();
                var pkValues = saveObjects.Select(it=>it.GetType().GetProperty(pkInfo.PropertyName).GetValue(it,null));
                if(existsObjects==null)
                    existsObjects=this.Context.Queryable<T>().In(pkValues).ToList();
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
                return saveObjects.Where(it=>!
                existsObjects.Any(e=>
                                 e.GetType().GetProperty(pkInfo.PropertyName).GetValue(e,null).ObjToString()
                                 == 
                                it.GetType().GetProperty(pkInfo.PropertyName).GetValue(it, null).ObjToString())).ToList();
            }
        }
        public List<T> updatObjects
        {
            get
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                List<T> result = new List<T>();
                var pks = GetPrimaryKeys();
                Check.Exception(pks.IsNullOrEmpty(), "Need primary key");
                Check.Exception(pks.Count() > 1, "Multiple primary keys are not supported");
                var pkInfo = this.EntityInfo.Columns.Where(it => it.IsIgnore == false).Where(it => it.DbColumnName.Equals(pks.First(), StringComparison.CurrentCultureIgnoreCase)).First();
                var pkValues = saveObjects.Select(it => it.GetType().GetProperty(pkInfo.PropertyName).GetValue(it, null));
                if (existsObjects == null)
                    existsObjects = this.Context.Queryable<T>().In(pkValues).ToList();
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
                return saveObjects.Where(it => 
                existsObjects.Any(e =>
                                 e.GetType().GetProperty(pkInfo.PropertyName).GetValue(e, null).ObjToString()
                                 ==
                                it.GetType().GetProperty(pkInfo.PropertyName).GetValue(it, null).ObjToString())).ToList();
            }
        }

        public IInsertable<T> insertable { get; set; }
        public IUpdateable<T> updateable { get; set; }

        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }

        #region Core
        public int ExecuteCommand()
        {
            LoadInsertable();
            LoadUpdateable();
            var insertCount = 0;
            var updateCount = 0;
            if (insertable != null)
            {
                insertCount = insertable.ExecuteCommand();
            }
            if (updateable != null)
            {
                updateCount = updateable.ExecuteCommand();
            }
            return updateCount + insertCount;
        }

        public T ExecuteReturnEntity()
        {
            LoadInsertable();
            LoadUpdateable();
            if (insertable != null)
                insertable.ExecuteCommandIdentityIntoEntity();
            if (updateable != null)
                updateable.ExecuteCommand();
            return saveObjects.First();
        }

        public List<T> ExecuteReturnList()
        {
            LoadInsertable();
            LoadUpdateable();
            if (insertable != null)
                insertable.ExecuteCommand();
            if (updateable != null)
                updateable.ExecuteCommand();
            return saveObjects;
        }
        #endregion
        #region Core Async
        public Task<int> ExecuteCommandAsync()
        {
            return Task.FromResult(ExecuteCommand());
        }

        public Task<T> ExecuteReturnEntityAsync()
        {
            return Task.FromResult(ExecuteReturnEntity());
        }

        public Task<List<T>> ExecuteReturnListAsync()
        {
            return Task.FromResult(ExecuteReturnList());
        }
        #endregion
        public ISaveable<T> InsertColumns(Expression<Func<T, object>> columns)
        {
            LoadInsertable();
            if (this.insertable != null)
            {
                this.insertable.InsertColumns(columns);
            }
            return this;
        }

        public ISaveable<T> EnableDiffLogEvent(object businessData = null)
        {
            LoadInsertable();
            LoadUpdateable();
            if (this.insertable != null)
            {
                this.insertable.EnableDiffLogEvent(businessData);
            }
            if (this.updateable != null)
            {
                this.updateable.EnableDiffLogEvent(businessData);
            }
            return this;
        }

        public ISaveable<T> InsertIgnoreColumns(Expression<Func<T, object>> columns)
        {
            LoadInsertable();
            if (this.insertable != null)
            {
                this.insertable.IgnoreColumns(columns);
            }
            return this;
        }

        public ISaveable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            LoadUpdateable();
            if (this.updateable != null)
            {
                this.updateable.UpdateColumns(columns);
            }
            return this;
        }

        public ISaveable<T> UpdateIgnoreColumns(Expression<Func<T, object>> columns)
        {
            LoadUpdateable();
            if (this.updateable != null)
            {
                this.updateable.IgnoreColumns(columns);
            }
            return this;
        }

        public ISaveable<T> UpdateWhereColumns(Expression<Func<T, object>> columns)
        {
            LoadUpdateable();
            if (this.updateable != null)
            {
                this.updateable.WhereColumns(columns);
            }
            return this;
        }
        protected virtual List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private void LoadInsertable()
        {
            var temp = insertObjects;
            if (insertable == null && temp.HasValue())
                insertable = this.Context.Insertable<T>(temp);
        }
        private void LoadUpdateable()
        {
            var temp = updatObjects;
            if (updateable == null && temp.HasValue())
                updateable = this.Context.Updateable<T>(temp);
        }

       
    }
}
