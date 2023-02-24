/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
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

                    if(index >= (column.ColumnName.Value.Length - 1) || (c != null && column.ColumnName.Value[index] != c)) {
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

            name = name.Replace('.', '_').Replace(' ', '_');

            if(name.Length > 0 && Char.IsNumber(name[0])) {
                name = "_" + name;
            }

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