using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteNavOptions
    {
        public bool ManyToManyIsDeleteA { get; set; }
        public bool ManyToManyIsDeleteB { get; set; }
    }
    public class DeleteNavRootOptions
    {
        public bool IsDiffLogEvent { get; set; }
        public object DiffLogBizData { get; set; }
    }
    public class InsertNavRootOptions
    {
        public string[] IgnoreColumns { get; set; }
        public string[] InsertColumns { get; set; }
        public bool IsDiffLogEvent { get; set; }
        public object DiffLogBizData { get; set; }
    }
    //public class InertNavRootOptions
    //{
    //    public string[] IgnoreColumns { get; set; }
    //    public string[] InsertColumns { get; set; }
    //}
    public class UpdateNavRootOptions
    {
        public string[] IgnoreColumns { get; set; }
        public string[] UpdateColumns { get; set; }
        public bool IsIgnoreAllNullColumns { get; set; }
        public bool IsInsertRoot { get; set; }
        public bool IsDisableUpdateRoot { get; set; }
        public bool IsDiffLogEvent { get; set; }
        public object  DiffLogBizData { get; set; }
        public string[] IgnoreInsertColumns { get; set; }
        public bool IsOptLock { get; set; }
 
    }
    public class UpdateNavOptions
    {
        public bool ManyToManyIsUpdateA { get; set; }
        public bool ManyToManyIsUpdateB { get; set; }
        public object ManyToManySaveMappingTemplate { get; set; }
        public bool ManyToManyEnableLogicDelete { get; set; }
        public bool OneToManyDeleteAll { get;  set; }
        public bool OneToManyEnableLogicDelete { get; set; }
        public bool OneToManyNoDeleteNull { get; set; }
        public bool OneToManyInsertOrUpdate { get; set; }
        public Expression RootFunc { get; set; }
        public Expression CurrentFunc { get; set; }
        public string[] IgnoreColumns { get; set; }
    }

    public class InsertNavOptions 
    {
        public bool OneToManyIfExistsNoInsert { get; set; }
        public bool ManyToManyNoDeleteMap { get; set; }
        public object ManyToManySaveMappingTemplate { get; set; }
    }
}
