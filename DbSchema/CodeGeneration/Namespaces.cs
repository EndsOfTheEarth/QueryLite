/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
using System;

namespace QueryLite.DbSchema.CodeGeneration {

    public sealed class Namespaces {

        public Namespaces(string baseNamespace, string tableNamespace, string classNamespace, string requestNamespace, string handlerNamespace) {
            BaseNamespace = FirstLetterUpperCase(baseNamespace);
            TableNamespace = FirstLetterUpperCase(tableNamespace);
            ClassNamespace = FirstLetterUpperCase(classNamespace);
            RequestNamespace = FirstLetterUpperCase(requestNamespace);
            HandlerNamespace = FirstLetterUpperCase(handlerNamespace);
        }

        private static string FirstLetterUpperCase(string value) {
            return !string.IsNullOrEmpty(value) ? $"{char.ToUpper(value[0])}{value[1..]}" : "";
        }
        public string BaseNamespace { get; set; }
        public string TableNamespace { get; set; }
        private string ClassNamespace { get; set; }
        private string RequestNamespace { get; set; }
        private string HandlerNamespace { get; set; }

        public string GetTableNamespace(SchemaName schema) {

            string schemaName = schema.Value;

            schemaName = FirstLetterUpperCase(schemaName);

            if(!IsDefaultSchema(schema)) {
                return $"{TableNamespace}.{schemaName}.Tables";
            }
            else {
                return TableNamespace;
            }
        }

        public string GetClassesNamespace(SchemaName schema) {

            string schemaName = schema.Value;

            schemaName = FirstLetterUpperCase(schemaName);

            if(!IsDefaultSchema(schema)) {
                return $"{ClassNamespace}.{schemaName}.Classes";
            }
            else {
                return TableNamespace;
            }
        }

        public string GetRequestsNamespace(SchemaName schema) {

            string schemaName = schema.Value;

            schemaName = FirstLetterUpperCase(schemaName);

            if(!IsDefaultSchema(schema)) {
                return $"{RequestNamespace}.{schemaName}.Requests";
            }
            else {
                return TableNamespace;
            }
        }

        public string GetHandlersNamespace(SchemaName schema) {

            string schemaName = schema.Value;

            schemaName = FirstLetterUpperCase(schemaName);

            if(!IsDefaultSchema(schema)) {
                return $"{HandlerNamespace}.{schemaName}.Handlers";
            }
            else {
                return TableNamespace;
            }
        }

        public static bool IsDefaultSchema(SchemaName schema) {
            return string.Equals(schema.Value, "dbo", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(schema.Value, "public", StringComparison.OrdinalIgnoreCase);
        }
    }
}