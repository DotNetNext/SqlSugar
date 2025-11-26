using System;

namespace SqlSugar.Migrations
{
    public abstract class Migration
    {
        public abstract void Up(SchemaBuilder schema);
        public abstract void Down(SchemaBuilder schema);

        public long Version
        {
            get
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MigrationAttribute));
                return attr?.Version ?? 0;
            }
        }

        public string Description
        {
            get
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MigrationAttribute));
                return attr?.Description ?? GetType().Name;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute : Attribute
    {
        public long Version { get; }
        public string Description { get; }

        public MigrationAttribute(long version, string description = null)
        {
            Version = version;
            Description = description;
        }
    }
}
