using System.Collections.Generic;

namespace QueryLite.PreparedQuery {

    public static class ColumnExtension {

        public static ACompiledCondition<ITEM> EQUALS<TYPE, ITEM>(this Column<TYPE> column, IParameter<TYPE, ITEM> parameter) where TYPE : notnull {
            return new CompiledParameterCondition<ITEM>(column, "=", parameter);
        }
        public static ACompiledCondition<ITEM> NOT_EQUALS<TYPE, ITEM>(this Column<TYPE> column, IParameter<TYPE, ITEM> parameter) where TYPE : notnull {
            return new CompiledParameterCondition<ITEM>(column, "!=", parameter);
        }

        public static ACompiledCondition<TYPE> IS_NULL<TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new CompiledIsNullCondition<TYPE>(column, @operator: "IS NULL");
        }
        public static ACompiledCondition<TYPE> IS_NOT_NULL<TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new CompiledIsNullCondition<TYPE>(column, @operator: "IS NOT NULL");
        }
    }

    public abstract class ACompiledCondition<ITEM> {

        public abstract string GetSql();
        public abstract void CollectParameters(List<IParameter<ITEM>> parameters);

        public ACompiledCondition<ITEM> AND(ACompiledCondition<ITEM> condition) {
            return new CompiledAndOrCondition<ITEM>(this, @operator: "AND", condition);
        }
        public ACompiledCondition<ITEM> OR(ACompiledCondition<ITEM> condition) {
            return new CompiledAndOrCondition<ITEM>(this, @operator: "AND", condition);
        }
    }

    public class CompiledParameterCondition<ITEM> : ACompiledCondition<ITEM> {

        public CompiledParameterCondition(IColumn column, string @operator, IParameter<ITEM>? parameter) {
            Column = column;
            Operator = @operator;
            Parameter = parameter;
        }
        public IColumn Column { get; }

        public string Operator { get; }
        public IParameter<ITEM>? Parameter { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

            if(Parameter != null) {
                parameters.Add(Parameter);
            }
        }

        public override string GetSql() {

            if(Parameter != null) {
                return $"{Column.Table.Alias}.{Column.ColumnName} {Operator} {Parameter.Name!}";
            }
            else {
                return $"{Column.Table.Alias}.{Column.ColumnName} {Operator}";
            }
        }
    }

    public class CompiledAndOrCondition<ITEM> : ACompiledCondition<ITEM> {

        public CompiledAndOrCondition(ACompiledCondition<ITEM> conditionA, string @operator, ACompiledCondition<ITEM> conditionB) {
            ConditionA = conditionA;
            Operator = @operator;
            ConditionB = conditionB;
        }

        public ACompiledCondition<ITEM> ConditionA { get; }
        public string Operator { get; }
        public ACompiledCondition<ITEM> ConditionB { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

            ConditionA.CollectParameters(parameters);
            ConditionB.CollectParameters(parameters);
        }
        public override string GetSql() {
            return $"({ConditionA.GetSql()} {Operator} {ConditionB.GetSql()})";
        }
    }

    public class CompiledIsNullCondition<ITEM> : ACompiledCondition<ITEM> {

        public CompiledIsNullCondition(IColumn column, string @operator) {
            Column = column;
            Operator = @operator;
        }
        public IColumn Column { get; }
        public string Operator { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

        }
        public override string GetSql() {
            return $"{Column.Table.Alias}.{Column.ColumnName}{Operator}";
        }
    }
}