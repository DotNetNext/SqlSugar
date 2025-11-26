using System;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Mark field as soft delete metadata
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SoftDeleteFieldAttribute : Attribute
    {
        public SoftDeleteFieldType FieldType { get; set; }

        public SoftDeleteFieldAttribute(SoftDeleteFieldType fieldType)
        {
            FieldType = fieldType;
        }
    }

    public enum SoftDeleteFieldType
    {
        IsDeleted = 1,
        DeletedAt = 2,
        DeletedBy = 3,
        DeletedReason = 4
    }
}
