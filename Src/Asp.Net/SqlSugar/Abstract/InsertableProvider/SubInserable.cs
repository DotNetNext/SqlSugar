using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SubInsertable<T> : ISubInsertable<T> where T : class, new()
    {
        internal EntityInfo Entity { get; set; }
        internal Dictionary<string, object> SubList { get; set; }
        internal SqlSugarProvider Context { get; set; }
        internal T [] InsertObjects { get; set; }
        internal InsertBuilder InsertBuilder { get; set; }
        internal string Pk { get; set; }

        public ISubInsertable<T> AddSubList(Expression<Func<T, object>> items)
        {
            if (InsertObjects != null&&InsertObjects.Count() > 0)
            {
                string subMemberName;
                object sublist;
                GetList(InsertObjects, items, out subMemberName, out sublist);
                if (!this.SubList.ContainsKey(subMemberName))
                {
                    this.SubList.Add(subMemberName, sublist);
                }
            }
            return this;
        }

        public object ExecuteReturnPrimaryKey()
        {
       
            if (InsertObjects != null && InsertObjects.Count()>0)
            {
                int count = 1;
                foreach (var InsertObject in InsertObjects)
                {
                    List<ConditionalModel> conModel = new List<ConditionalModel>();
                    int id = this.Context.Insertable(InsertObject).ExecuteReturnIdentity();
                    object pkValue = null;
                    var qureyable = this.Context.Queryable<T>();
                    if (id.ObjToInt() == 0)
                    {
                        var primaryProperty = this.Entity.Columns.FirstOrDefault(it =>
                                                                                        it.PropertyName.Equals(this.Pk, StringComparison.CurrentCultureIgnoreCase) ||
                                                                                        it.DbColumnName.Equals(this.Pk, StringComparison.CurrentCultureIgnoreCase)
                                                                                     );
                        pkValue = primaryProperty.PropertyInfo.GetValue(InsertObject);
                        qureyable.In(pkValue);
                    }
                    else
                    {
                        qureyable.In(id);
                        pkValue = id;
                    }
                    var data = qureyable.First();
                    foreach (var item in this.SubList)
                    {

                        Dictionary<string, object> insertDictionary = new Dictionary<string, object>();
                        if (item.Value == null)
                        {
                            continue;
                        }
                        EntityInfo subEntity = null;
                        if (item.Value is IEnumerable<object>)
                        {
                            var list = item.Value as IEnumerable<object>;
                            if (list.Count() == 0)
                            {
                                continue;
                            }
                            var type = list.First().GetType();
                            this.Context.InitMappingInfo(type);
                            subEntity = this.Context.EntityMaintenance.GetEntityInfo(type);
                            foreach (var sbItem in list)
                            {
                                SetItems(insertDictionary, sbItem, subEntity, item.Key, pkValue);
                            }
                        }
                        else if (item.Value.GetType().IsClass())
                        {
                            var type = item.Value.GetType();
                            this.Context.InitMappingInfo(type);
                            subEntity = this.Context.EntityMaintenance.GetEntityInfo(type);
                            SetItems(insertDictionary, item.Value, subEntity, item.Key, pkValue);
                        }
                        count += this.Context.Insertable(insertDictionary).AS(subEntity.DbTableName).ExecuteCommand();
                    }
                }
                return count;
            }
            else
            {
                return 0;
            }
     
        }
        public async Task<object> ExecuteReturnPrimaryKeyAsync()
        {

            if (InsertObjects != null && InsertObjects.Count() > 0)
            {
                int count = 1;
                foreach (var InsertObject in InsertObjects)
                {
                    List<ConditionalModel> conModel = new List<ConditionalModel>();
                    int id = await this.Context.Insertable(InsertObject).ExecuteReturnIdentityAsync();
                    object pkValue = null;
                    var qureyable = this.Context.Queryable<T>();
                    if (id.ObjToInt() == 0)
                    {
                        var primaryProperty = this.Entity.Columns.FirstOrDefault(it =>
                                                                                        it.PropertyName.Equals(this.Pk, StringComparison.CurrentCultureIgnoreCase) ||
                                                                                        it.DbColumnName.Equals(this.Pk, StringComparison.CurrentCultureIgnoreCase)
                                                                                     );
                        pkValue = primaryProperty.PropertyInfo.GetValue(InsertObject);
                        qureyable.In(pkValue);
                    }
                    else
                    {
                        qureyable.In(id);
                        pkValue = id;
                    }
                    var data =await qureyable.FirstAsync();
                    foreach (var item in this.SubList)
                    {

                        Dictionary<string, object> insertDictionary = new Dictionary<string, object>();
                        if (item.Value == null)
                        {
                            continue;
                        }
                        EntityInfo subEntity = null;
                        if (item.Value is IEnumerable<object>)
                        {
                            var list = item.Value as IEnumerable<object>;
                            if (list.Count() == 0)
                            {
                                continue;
                            }
                            var type = list.First().GetType();
                            this.Context.InitMappingInfo(type);
                            subEntity = this.Context.EntityMaintenance.GetEntityInfo(type);
                            foreach (var sbItem in list)
                            {
                                SetItems(insertDictionary, sbItem, subEntity, item.Key, pkValue);
                            }
                        }
                        else if (item.Value.GetType().IsClass())
                        {
                            var type = item.Value.GetType();
                            this.Context.InitMappingInfo(type);
                            subEntity = this.Context.EntityMaintenance.GetEntityInfo(type);
                            SetItems(insertDictionary, item.Value, subEntity, item.Key, pkValue);
                        }
                        count +=await this.Context.Insertable(insertDictionary).AS(subEntity.DbTableName).ExecuteCommandAsync();
                    }
                }
                return count;
            }
            else
            {
                return 0;
            }

        }
        public void GetList(T[] inserts,Expression<Func<T, object>> items, out string subMemberName, out object sublist)
        {
            var lambdaExpression = (items as LambdaExpression).Body;
            if (lambdaExpression is UnaryExpression)
            {
                lambdaExpression = (lambdaExpression as UnaryExpression).Operand;
            }
            MemberExpression subMemberException = lambdaExpression as MemberExpression;
            subMemberName = subMemberException.Member.Name;
            MemberExpression listMember = null;
            sublist = null;
            if (subMemberException.Expression is MethodCallExpression)
            {
                listMember = (subMemberException.Expression as MethodCallExpression).Arguments.First() as MemberExpression;

            }
            else
            {
                listMember = (subMemberException.Expression as MemberExpression);
            }
            if (listMember == null)
            {
                listMember = (items as LambdaExpression).Body as MemberExpression;
                subMemberName = Guid.NewGuid().ToString();
            }
            sublist = inserts.First().GetType().GetProperty(listMember.Member.Name).GetValue(inserts.First());
        }
        private void SetItems(Dictionary<string, object> insertDictionary, object sbItem, EntityInfo subEntity,string key,object pkValue)
        {
            foreach (var item in subEntity.Columns)
            {
                if (item.IsIdentity||item.IsIgnore)
                    continue;
                if (!string.IsNullOrEmpty(item.OracleSequenceName)&&this.Context.CurrentConnectionConfig.DbType==DbType.Oracle)
                {
                    var value = "{SugarSeq:=}"+item.OracleSequenceName+ ".nextval{SugarSeq:=}";
                    insertDictionary.Add(item.DbColumnName, value);
                    continue;
                }
                if (item.PropertyInfo.Name == key)
                {
                    insertDictionary.Add(item.DbColumnName, pkValue);
                }
                else
                {
                    insertDictionary.Add(item.DbColumnName, item.PropertyInfo.GetValue(sbItem));
                }
            }
        }
    }
}
