using System;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Mark entity as soft-deletable
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SoftDeleteAttribute : Attribute
    {
        public string DeletedFieldName { get; set; } = "IsDeleted";
        public string DeletedAtFieldName { get; set; } = "DeletedAt";
        public string DeletedByFieldName { get; set; } = "DeletedBy";
        public string DeletedReasonFieldName { get; set; } = "DeletedReason";
        public bool EnableCascadeDelete { get; set; } = false;
    }
}
