using System.Text;
using System;

namespace QueryLite.DbSchema.CodeGeneration {
    
    /// <summary>
    /// This class determines if all the columns of a particular table share a common prefix
    /// </summary>
    public sealed class TablePrefix {

        public string Prefix { get; private set; }

        public TablePrefix(string prefix) {
            Prefix = prefix;
        }

        public TablePrefix(DatabaseTable table) {

            if(table.Columns.Count <= 1) {
                Prefix = string.Empty;
                return;
            }

            bool stop = false;

            StringBuilder prefix = new StringBuilder();

            for(int index = 0; index < 10; index++) {   // Limit prefix up to 10 characters

                char? c = null;

                foreach(DatabaseColumn column in table.Columns) {

                    if(index >= column.ColumnName.Value.Length || (c != null && column.ColumnName.Value[index] != c)) {
                        stop = true;
                        break;
                    }
                    else {
                        c = column.ColumnName.Value[index];
                    }
                }
                if(!stop && c != null) {
                    prefix.Append(c.Value);
                }
                else {
                    break;
                }
            }
            Prefix = prefix.Length > 1 ? prefix.ToString() : string.Empty;
        }

        public string GetColumnName(string columnName) {

            string name = columnName;

            if(Prefix.Length > 0 && columnName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)) {
                name = columnName[Prefix.Length..];
            }

            string first = "" + name[0];

            if(first != first.ToUpper() && name.Length > 1) {
                name = string.Concat(first.ToUpper(), name.AsSpan(1));
            }
            return name;
        }
    }
}