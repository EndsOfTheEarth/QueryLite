using System;

namespace QueryLite {

    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryAttribute<TABLE> : Attribute where TABLE : ATable {

        public MatchOn MatchOn { get; init; }
        public string RepositoryName { get; } = "";

        public RepositoryAttribute(MatchOn matchOn, string repositoryName = "") {
            MatchOn = matchOn;
            RepositoryName = repositoryName;
        }
    }
}