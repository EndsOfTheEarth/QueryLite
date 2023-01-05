
namespace QueryLite.DbSchema.CodeGeneration {

    public sealed class Namespaces {

        public Namespaces(string baseNamespace, string tableNamespace, string classNamespace) {
            BaseNamespace = baseNamespace;
            TableNamespace = tableNamespace;
            ClassNamespace = classNamespace;
        }
        public string BaseNamespace { get; set; }
        public string TableNamespace { get; set; }
        public string ClassNamespace { get; set; }
    }
}