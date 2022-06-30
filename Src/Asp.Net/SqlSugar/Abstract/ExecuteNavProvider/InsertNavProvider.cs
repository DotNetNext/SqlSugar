using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class InsertNavProvider<Root,T> where T : class,new() where Root:class,new()
    {

        public List<Root> _Roots { get;  set; }
        public List<object> _ParentList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public SqlSugarProvider _Context { get;   set; }


        public InsertNavProvider<Root, Root> AsNav()
        {
            return new InsertNavProvider<Root, Root> { 
             _Context = _Context,  
             _ParentEntity = null,
              _ParentList=null,
               _Roots= _Roots
            };
        }

        public InsertNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                InsertOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                InsertOneToMany<TChild>(name, nav);
            }
            else
            {
                InsertManyToMany<TChild>(name, nav);
            }
            return GetResult<TChild>();
        }
 
        private static bool IsDefaultValue(object pvValue)
        {
            return pvValue == null || pvValue == UtilMethods.GetDefaultValue(pvValue.GetType());
        }
        private void InitParentList()
        {
            if (_ParentList == null)
            {
                _ParentList = GetParentList(_Roots).Cast<object>().ToList();
            }
        }

        private InsertNavProvider<Root, TChild> GetResult<TChild>() where TChild : class, new()
        {
            return new InsertNavProvider<Root, TChild>()
            {
                _Context = this._Context,
                _ParentEntity = this._ParentEntity,
                _ParentList = this._ParentList,
                _Roots = this._Roots
            };
        }

        private List<Type> GetParentList<Type>(List<Type> datas) where Type : class ,new()
        {
            List<Type> result = new List<Type>();
            this._Context.InitMappingInfo<Type>();
            var entity = this._Context.EntityMaintenance.GetEntityInfo<Type>();
            var isIdentity = entity.Columns.Where(it=>it.IsIdentity).Any();
            if (isIdentity)
            {
                InsertIdentity(datas);
            }
            else 
            {
                this._Context.Insertable(datas).ExecuteCommand();
            }
            this._ParentEntity = entity;
            result = datas;
            return result;
        }

        private void InsertIdentity<Type>(List<Type> datas) where Type : class, new()
        {
            foreach (var item in datas)
            {
                this._Context.Insertable(item).ExecuteCommandIdentityIntoEntity();
            }
        }
    }
}
