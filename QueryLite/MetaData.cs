using System;

namespace QueryLite {

    /// <summary>
    /// Description attribute. Used to add human readable descriptions to table classes and table columns. These attrubutes are used to auto generate schema documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class DescriptionAttribute : Attribute {

        public string Description { get; }

        public DescriptionAttribute(string description) {
            Description = description ?? string.Empty;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute {

        public string Name { get; }

        public PrimaryKeyAttribute(string name) {
            Name = name;
        }
    }

    public interface IForeignKeyAttribute {

        string Name { get; }
        Type PrimaryKeyTable { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute<TABLE> : Attribute, IForeignKeyAttribute where TABLE : ITable {

        public string Name { get; }
        public Type PrimaryKeyTable { get; } = typeof(TABLE);

        public ForeignKeyAttribute(string name) {
            Name = name;
        }
    }
}