﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SubInsertable<T> : ISubInsertable<T> where T : class, new()
    {
        internal EntityInfo Entity { get; set; }
        internal List<SubInsertTreeExpression> SubList { get; set; }
        internal SqlSugarProvider Context { get; set; }
        internal T[] InsertObjects { get; set; }
        internal InsertBuilder InsertBuilder { get; set; }
        internal string Pk { get; set; }

        public ISubInsertable<T> AddSubList(Expression<Func<T, object>> items)
        {
            if (this.SubList == null)
                this.SubList = new List<SubInsertTreeExpression>();
            this.SubList.Add(new SubInsertTreeExpression() { Expression = items });
            return this;
        }
        public ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree)
        {
            try
            {
                var lamda = (tree as LambdaExpression);
                var memInit = lamda.Body as MemberInitExpression;
                if (memInit.Bindings != null)
                {

                    MemberAssignment memberAssignment = (MemberAssignment)memInit.Bindings[0];
                    SubList.Add(new SubInsertTreeExpression()
                    {
                        Expression = memberAssignment.Expression,
                        Childs = GetSubInsertTree(((MemberAssignment)memInit.Bindings[1]).Expression)
                    });
                }
            }
            catch  
            {
                Check.Exception(true, tree.ToString() + " format error ");
            }
            return this;
        }

        private List<SubInsertTreeExpression> GetSubInsertTree(Expression expression)
        {
            List<SubInsertTreeExpression> resul = new List<SubInsertTreeExpression>();
          
            if (expression is ListInitExpression)
            {
                ListInitExpression exp = expression as ListInitExpression;
                foreach (var item in exp.Initializers)
                {
                    SubInsertTreeExpression tree = new SubInsertTreeExpression();
                    var memInit = item.Arguments[0] as MemberInitExpression;
                    if (memInit.Bindings != null)
                    {
                        MemberAssignment memberAssignment = (MemberAssignment)memInit.Bindings[0];
                        tree.Expression = memberAssignment.Expression;
                        if (memInit.Bindings.Count > 1)
                        {
                            tree.Childs = GetSubInsertTree(((MemberAssignment)memInit.Bindings[1]).Expression);
                        }
                    }
                    resul.Add(tree);
                }
            }
            else
            {
                
            }
            return resul;
        }

        [Obsolete("use ExecuteCommand")]
        public object ExecuteReturnPrimaryKey() 
        {
            return ExecuteCommand();
        }

        public async Task<object> ExecuteCommandAsync()
        {
            object resut = 0;
            await Task.Run(() =>
            {
                resut= ExecuteCommand();
            });
            return resut;
        }
        public object ExecuteCommand()
        {
            var isNoTrean = this.Context.Ado.Transaction == null;
            try
            {
                if (isNoTrean)
                    this.Context.Ado.BeginTran();

                var result = Execute();

                if (isNoTrean)
                    this.Context.Ado.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                if (isNoTrean)
                    this.Context.Ado.RollbackTran();
                throw ex;
            }
        }

        private object Execute()
        {
            if (InsertObjects != null && InsertObjects.Count() > 0)
            {
                var isIdEntity = IsIdEntity(this.Entity);
                if (!isIdEntity)
                {
                    this.Context.Insertable(InsertObjects).ExecuteCommand();
                }
                foreach (var InsertObject in InsertObjects)
                {
                    int id = 0;
                    if (isIdEntity)
                    {
                        id = this.Context.Insertable(InsertObject).ExecuteReturnIdentity();
                        this.Entity.Columns.First(it => it.IsIdentity || it.OracleSequenceName.HasValue()).PropertyInfo.SetValue(InsertObject, id);
                    }
                    var pk = GetPrimaryKey(this.Entity, InsertObject, id);
                    AddChildList(this.SubList, InsertObject, pk);
                }
                return InsertObjects.Count();
            }
            else
            {
                return 0;
            }
        }
        [Obsolete("use ExecuteCommandAsync")]
        public Task<object> ExecuteReturnPrimaryKeyAsync()
        {
            return Task.FromResult(ExecuteReturnPrimaryKey());
        }

        private bool IsIdEntity(EntityInfo entity)
        {
            return entity.Columns.Where(it => it.IsIdentity||it.OracleSequenceName.HasValue()).Count() > 0;
        }

        private void AddChildList(List<SubInsertTreeExpression> items, object insertObject, object pkValue)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    MemberExpression subMemberException;
                    string subMemberName = GetMemberName(item, out subMemberException);
                    string childName = GetChildName(item, subMemberException);
                    var childListProperty = insertObject.GetType().GetProperty(childName);
                    if (childListProperty == null)
                    {
                        childName = subMemberName;
                        childListProperty = insertObject.GetType().GetProperty(childName);
                    }
                    var childList = childListProperty.GetValue(insertObject);
                    if (childList != null)
                    {
                        if (!(childList is IEnumerable<object>))
                        {
                            childList = new List<object>() { childList };
                        }
                        if (!string.IsNullOrEmpty(subMemberName) &&subMemberName!=childName)
                        {
                            foreach (var child in childList as IEnumerable<object>)
                            {
                                child.GetType().GetProperty(subMemberName).SetValue(child, pkValue);
                            }
                        }
                        if (!(childList as IEnumerable<object>).Any())
                        {
                            continue;
                        }
                        var type = (childList as IEnumerable<object>).First().GetType();
                        this.Context.InitMappingInfo(type);
                        var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type);
                        var isIdentity = IsIdEntity(entityInfo);
                        var tableName = entityInfo.DbTableName;
                        List<Dictionary<string, object>> insertList = new List<Dictionary<string, object>>();
                        var entityList = (childList as IEnumerable<object>).ToList();
                        foreach (var child in entityList)
                        {
                            insertList.Add(GetInsertDictionary(child, entityInfo));
                        }
                        if (!isIdentity)
                        {
                            this.Context.Insertable(insertList).AS(tableName).ExecuteCommand();
                        }
                        int i = 0;
                        foreach (var insert in insertList)
                        {
                            int id = 0;
                            if (isIdentity)
                            {
                                if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                                {
                                    var sqlobj = this.Context.Insertable(insert).AS(tableName).ToSql();
                                    id = this.Context.Ado.GetInt(sqlobj.Key+ " returning " + this.InsertBuilder.Builder.GetTranslationColumnName(entityInfo.Columns.First(it=>isIdentity).DbColumnName), sqlobj.Value);
                                }
                                else
                                {
                                    id = this.Context.Insertable(insert).AS(tableName).ExecuteReturnIdentity();
                                }
                                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle&&id==0)
                                {
                                    var seqName=entityInfo.Columns.First(it => it.OracleSequenceName.HasValue())?.OracleSequenceName;
                                    id = this.Context.Ado.GetInt("select "+seqName+".currval from dual");
                                }
                            }
                            var entity = entityList[i];
                            var pk = GetPrimaryKey(entityInfo,entity, id);
                            AddChildList(item.Childs, entity, pk);
                            ++i;
                        }
                    }
                }
            }
        }
        private Dictionary<string, object> GetInsertDictionary(object insetObject, EntityInfo subEntity)
        {
            Dictionary<string, object> insertDictionary = new Dictionary<string, object>();
            foreach (var item in subEntity.Columns)
            {
                if (item.IsIdentity || item.IsIgnore)
                {

                }
                else if (!string.IsNullOrEmpty(item.OracleSequenceName) && this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                {
                    var value = "{SugarSeq:=}" + item.OracleSequenceName + ".nextval{SugarSeq:=}";
                    insertDictionary.Add(item.DbColumnName, value);
                    continue;
                }
                else
                {
                    var value = item.PropertyInfo.GetValue(insetObject);
                    if (value == null&&this.Context.CurrentConnectionConfig.DbType==DbType.PostgreSQL) 
                    {
                       var underType= UtilMethods.GetUnderType(item.PropertyInfo.PropertyType);
                        if (underType == UtilConstants.DateType)
                        {
                            value = SqlDateTime.Null;
                        }
                    }
                    insertDictionary.Add(item.DbColumnName, value);
                }
            }
            return insertDictionary;
        }
        private static string GetChildName(SubInsertTreeExpression item, MemberExpression subMemberException)
        {
            string childName;
            MemberExpression listMember = null;
            if (subMemberException.Expression is MethodCallExpression)
            {
                listMember = (subMemberException.Expression as MethodCallExpression).Arguments.First() as MemberExpression;

            }
            else
            {
                listMember = (subMemberException.Expression as MemberExpression);
            }
            if (listMember == null&& item.Expression is LambdaExpression)
            {
                listMember = (item.Expression as LambdaExpression).Body as MemberExpression;
            }
            if (listMember == null && item.Expression is MemberExpression)
            {
                listMember =  item.Expression  as MemberExpression;
            }
            childName = listMember.Member.Name;
            return childName;
        }

        private static string GetMemberName(SubInsertTreeExpression item, out MemberExpression subMemberException)
        {
            string subMemberName = null;
            Expression lambdaExpression;
            if (item.Expression is LambdaExpression)
            {
                lambdaExpression = (item.Expression as LambdaExpression).Body;
            }
            else
            {
                lambdaExpression = item.Expression;
            }
            if (lambdaExpression is UnaryExpression)
            {
                lambdaExpression = (lambdaExpression as UnaryExpression).Operand;
            }
            subMemberException = lambdaExpression as MemberExpression;
            subMemberName = subMemberException.Member.Name;
            return subMemberName;
        }

        private object GetPrimaryKey(EntityInfo entityInfo,object InsertObject, int id)
        {
            object pkValue;
            if (id.ObjToInt() == 0)
            {
                var primaryProperty = entityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
                Check.Exception(primaryProperty == null, entityInfo.EntityName + " no primarykey");
                pkValue = primaryProperty.PropertyInfo.GetValue(InsertObject);
            }
            else
            {
                pkValue = id;
            }

            return pkValue;
        }
    }
}
