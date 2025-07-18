
namespace QueryLite {

    [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
    public class RepositoryAttribute<TABLE> : global::System.Attribute where TABLE : global::QueryLite.ATable {

        public global::QueryLite.MatchOn MatchOn { get; init; }
        public string RepositoryName { get; } = "";

        public RepositoryAttribute(global::QueryLite.MatchOn matchOn, string repositoryName = "") {
            MatchOn = matchOn;
            RepositoryName = repositoryName;
        }
    }
}