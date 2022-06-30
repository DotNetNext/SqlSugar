using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class InsertNavProvider<Root,T> where T : class,new() where Root:class,new()
    {

        public List<Root> Roots { get;  set; }
        public List<object> ParentList { get; set; }
        public EntityInfo ParentEntity { get; set; }
        public SqlSugarProvider Context { get;   set; }

        public InsertNavProvider<Root,TChild> ThenInclude<TChild>(Expression<Func<T,TChild>> expression) where TChild : class, new()
        {
            if (ParentList == null) 
            {
                ParentList = GetParentList(Roots).Cast<object>().ToList();
            }
            var name=ExpressionTool.GetMemberName(expression);
            var nav = this.ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null) 
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this.ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                InsertOneToOne<TChild>(ParentList,this.ParentEntity, name,nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                InsertOneToMany();
            }
            else 
            {
                InsertManyToMany();
            }
            return null;
        }

        private void InsertManyToMany()
        {
             
        }

        private void InsertOneToMany()
        {
       
        }

        private void InsertOneToOne<TChild>(List<object> parentList, EntityInfo parentEntity, string name, EntityColumnInfo nav)
        {
            var parentColumn = parentEntity.Columns.FirstOrDefault(it => it.PropertyName==nav.Navigat.Name);
            this.ParentEntity = this.Context.EntityMaintenance.GetEntityInfo<TChild>();
            var pkColumn = ParentEntity.Columns.FirstOrDefault(it=>it.IsPrimarykey==true);
            if (nav.Navigat.Name2.HasValue()) 
            {
                pkColumn = ParentEntity.Columns.FirstOrDefault(it => it.PropertyName==nav.Navigat.Name2);
            }
            Check.Exception(pkColumn == null, $" Navigate {parentEntity.EntityName} : {name} is error ", $"导航实体 {parentEntity.EntityName} 属性 {name} 配置错误");
            List<object> childList = new List<object>();
            foreach (var parent in parentList) 
            {
                var childItems=(TChild)nav.PropertyInfo.GetValue(parent);
                if (childItems != null)
                {
                    var pkValue = pkColumn.PropertyInfo.GetValue(childItems);
                    var pvValue = parentColumn.PropertyInfo.GetValue(parent);
                    if (IsDefaultValue(pvValue))
                    {
                        pvValue = pkValue;
                    }
                    if (IsDefaultValue(pvValue))
                    {
                        
                    }
                }
            }
            parentList = childList;
        }

        private static bool IsDefaultValue(object pvValue)
        {
            return pvValue == null || pvValue == UtilMethods.GetDefaultValue(pvValue.GetType());
        }

        private List<Type> GetParentList<Type>(List<Type> datas) where Type : class ,new()
        {
            List<Type> result = new List<Type>();
            this.Context.InitMappingInfo<Type>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<Type>();
            var isIdentity = entity.Columns.Where(it=>it.IsIdentity).Any();
            if (isIdentity)
            {
                foreach (var item in datas)
                {
                    this.Context.Insertable(datas).ExecuteCommandIdentityIntoEntity();
                }
            }
            else 
            {
                this.Context.Insertable(datas).ExecuteCommand();
            }
            this.ParentEntity = entity;
            result = datas;
            return result;
        }

        public InsertNavProvider<Root,Root> AsNav()
        {
            return null;
        }
    }
}
