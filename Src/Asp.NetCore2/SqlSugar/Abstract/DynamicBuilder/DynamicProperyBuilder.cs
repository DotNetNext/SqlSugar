﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace SqlSugar 
{

    public class DynamicProperyBuilder
    {
        private bool IsCache = false;
        public static DynamicProperyBuilder CopyNew() 
        {
            return new DynamicProperyBuilder();
        }
        public DynamicBuilder baseBuilder;
        public DynamicProperyBuilder CreateProperty(string propertyName, Type properyType, SugarColumn column=null,bool isSplitField=false, Navigate navigate=null)
        {
            if (column == null) 
            {
                column = new SugarColumn()
                {
                     ColumnName=propertyName
                };
            }
            PropertyMetadata addItem = new PropertyMetadata();
            addItem.Name = propertyName;
            addItem.Type = properyType;
            addItem.CustomAttributes = new List<CustomAttributeBuilder>() { baseBuilder.GetProperty(column) };
            if (navigate != null) 
            {
                addItem.CustomAttributes.Add(BuildNavigateAttribute(navigate));
            }
            baseBuilder.propertyAttr.Add(addItem);
            if (isSplitField) 
            {
                addItem.CustomAttributes.Add(baseBuilder.GetSplitFieldAttr(new SplitFieldAttribute()));
            }
            return this;
        }
        public DynamicProperyBuilder WithCache(bool isCache=true)
        {
            IsCache = isCache;
            return this;
        }
        public Type BuilderType()
        {
            if (IsCache)
            {
                var key = baseBuilder.entityName + string.Join("_", baseBuilder.propertyAttr.Select(it => it.Name + it.Type.Name));
                return  new ReflectionInoCacheService().GetOrCreate(key,() =>
                {
                    var result = DynamicBuilderHelper.CreateDynamicClass(baseBuilder.entityName, baseBuilder.propertyAttr, TypeAttributes.Public, baseBuilder.entityAttr, baseBuilder.baseType, baseBuilder.interfaces);
                    return result;
                });
            }
            else
            {
                var result = DynamicBuilderHelper.CreateDynamicClass(baseBuilder.entityName, baseBuilder.propertyAttr, TypeAttributes.Public, baseBuilder.entityAttr, baseBuilder.baseType, baseBuilder.interfaces);
                return result;
            }
        }

        #region BuilderTypes
        public Tuple<Type, Type> BuilderTypes(DynamicProperyBuilder dynamicBuilderB)
        {
            if (IsCache)
            {
                var key1 = baseBuilder.entityName + string.Join("_", baseBuilder.propertyAttr.Select(it => it.Name + it.Type.Name));
                var key2 = dynamicBuilderB.baseBuilder.entityName + string.Join("_", dynamicBuilderB.baseBuilder.propertyAttr.Select(it => it.Name + it.Type.Name));
                return new ReflectionInoCacheService().GetOrCreate(key1 + key2, () =>
                {
                    Tuple<Type, Type> result = GetBuilderTypes(dynamicBuilderB);
                    return result;
                });
            }
            else
            {
                Tuple<Type, Type> result = GetBuilderTypes(dynamicBuilderB);
                return result;
            }
        }
        private Tuple<Type,Type> GetBuilderTypes(DynamicProperyBuilder dynamicBuilderB)
        {
            DynamicProperyBuilder dynamicBuilderA = this;
            TypeBuilder typeBuilderA = EmitTool.CreateTypeBuilder(dynamicBuilderA.baseBuilder.entityName, TypeAttributes.Public, dynamicBuilderA.baseBuilder.baseType, dynamicBuilderA.baseBuilder.interfaces);
            TypeBuilder typeBuilderB = EmitTool.CreateTypeBuilder(dynamicBuilderB.baseBuilder.entityName, TypeAttributes.Public, dynamicBuilderB.baseBuilder.baseType, dynamicBuilderB.baseBuilder.interfaces);
            DynamicBuilderHelper.CreateDynamicClass(typeBuilderA, typeBuilderB, dynamicBuilderA.baseBuilder.propertyAttr, dynamicBuilderA.baseBuilder.entityAttr);
            DynamicBuilderHelper.CreateDynamicClass(typeBuilderB, typeBuilderA, dynamicBuilderB.baseBuilder.propertyAttr, dynamicBuilderB.baseBuilder.entityAttr);
            return new Tuple<Type, Type>(typeBuilderB.CreateTypeInfo().AsType(), typeBuilderA.CreateTypeInfo().AsType());
        } 
        #endregion

        public  CustomAttributeBuilder BuildNavigateAttribute(Navigate navigate)
        {
            NavigateType navigatType = navigate.NavigatType;
            string name = navigate.Name;
            string name2 = navigate.Name2;
            string whereSql = navigate.WhereSql;
            Type mappingTableType = navigate.MappingType;
            string typeAiD = navigate.MappingAId;
            string typeBId = navigate.MappingBId; 
            var queryPropertyNames = navigate.QueryPropertyNames;
            ConstructorInfo constructor;
            object[] constructorArgs;

            if (mappingTableType != null && typeAiD != null && typeBId != null)
            {
                constructor = typeof(Navigate).GetConstructor(new Type[] { typeof(Type), typeof(string), typeof(string), typeof(string), typeof(string[]) });
                constructorArgs = new object[] { mappingTableType, typeAiD, typeBId, whereSql , queryPropertyNames };
            }
            else if (!string.IsNullOrEmpty(whereSql))
            {
                constructor = typeof(Navigate).GetConstructor(new Type[] { typeof(NavigateType), typeof(string), typeof(string), typeof(string), typeof(string[]) });
                constructorArgs = new object[] { navigatType, name, name2, whereSql, queryPropertyNames };
            }
            else if (!string.IsNullOrEmpty(name2))
            {
                constructor = typeof(Navigate).GetConstructor(new Type[] { typeof(NavigateType), typeof(string), typeof(string), typeof(string[]) });
                constructorArgs = new object[] { navigatType, name, name2, queryPropertyNames };
            }
            else
            {
                constructor = typeof(Navigate).GetConstructor(new Type[] { typeof(NavigateType), typeof(string), typeof(string[]) });
                constructorArgs = new object[] { navigatType, name, queryPropertyNames };
            }

            return new CustomAttributeBuilder(constructor, constructorArgs);
        }

    }
}
