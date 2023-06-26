﻿/*
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
using System;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class FluentValidationGenerator {

        public static CodeBuilder GenerateFluentValidationCode(IDatabase database, DatabaseTable table, TablePrefix prefix, bool useIdentifiers, Namespaces namespaces) {

            CodeBuilder classCode = new CodeBuilder();

            classCode.Append($"namespace {namespaces.GetClassesNamespace(table.Schema)} {{").EndLine().EndLine();
            classCode.Indent(1).Append("using System;").EndLine();
            classCode.Indent(1).Append("using FluentValidation;").EndLine();
            classCode.Indent(1).Append($"using {namespaces.TableNamespace};").EndLine();

            string className = CodeHelper.GetTableName(table, includePostFix: false);

            classCode.EndLine();

            CodeBuilder validateCode = new CodeBuilder();

            string validateClassName = className + "Validator";

            validateCode.Indent(1).Append($"public sealed class {validateClassName} : AbstractValidator<{className}> {{").EndLine();
            validateCode.EndLine();

            bool includeIsNew = false;

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsAutoGenerated) {
                    includeIsNew = true;
                    break;
                }
            }

            validateCode.Indent(2).Append($"public {validateClassName}({(includeIsNew ? "bool isNew" : "")}) {{").EndLine();
            validateCode.EndLine();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IUnsupportedType)) || column.DataType.DotNetType.IsAssignableTo(typeof(IGeography))) {
                    continue;
                }

                CodeHelper.GetColumnName(table, column, useIdentifiers: useIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string _, out bool isKeyColumn);

                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: null);

                string columnNameFirstLetterLowerCase = columnName.Length > 1 ? string.Concat(columnName[0].ToString().ToLower(), columnName.AsSpan(1)) : columnName;

                if(!column.IsNullable) {

                    if(!column.DataType.DotNetType.IsValueType) {
                        validateCode.Indent(3).Append($"RuleFor(_x => _x.{columnName}).NotNull();").EndLine();
                    }

                    if(isKeyColumn) {

                        if(column.IsAutoGenerated) {
                            validateCode.Indent(3).Append($"RuleFor(_x => _x.{columnName}).Must({columnNameFirstLetterLowerCase} => !{columnNameFirstLetterLowerCase}.IsValid).When(_x => isNew).WithMessage(\"{columnName} must not be valid when creating a record as it is auto generated\");").EndLine();
                            validateCode.Indent(3).Append($"RuleFor(_x => _x.{columnName}).Must({columnNameFirstLetterLowerCase} => {columnNameFirstLetterLowerCase}.IsValid).When(_x => !isNew).WithMessage(\"{columnName} must be valid\");").EndLine().EndLine();
                        }
                        else {
                            validateCode.Indent(3).Append($"RuleFor(_x => _x.{columnName}).Must({columnNameFirstLetterLowerCase} => {columnNameFirstLetterLowerCase}.IsValid).WithMessage(\"{columnName} must be valid\");").EndLine();
                        }
                    }
                }
                else if(column.IsNullable && isKeyColumn) {
                    validateCode.Indent(3).Append($"RuleFor(_x => _x.{columnName}).Must({columnNameFirstLetterLowerCase} => {columnNameFirstLetterLowerCase}!.Value.IsValid).When(_x => _x.{columnName} != null).WithMessage(\"{columnName} must be valid\");").EndLine();
                }

                CodeBuilder rule = new CodeBuilder();
                rule.Indent(3).Append($"RuleFor(_x => _x.{columnName})");

                bool hasRule = false;

                //column.DataType.DotNetType.

                //if(!column.IsAutoGenerated && !column.IsNullable && !column.DataType.DotNetType.IsValueType) {
                //    rule.Append(".NotNull()");
                //    hasRule = true;
                //}

                bool addBeginAndEndLine = false;

                if(column.Length != null) {

                    if(dotNetType == typeof(string) && !isKeyColumn) {
                        rule.Append($".Length(min:0, max: {CodeHelper.GetTableName(table, includePostFix: true)}.Instance.{columnName}.Length!.Value)");
                    }
                    else {

                        rule.EndLine().Indent(4);
                        rule.Append($".Must({columnNameFirstLetterLowerCase} => ");

                        string maxLength = $"{CodeHelper.GetTableName(table, includePostFix: true)}.Instance.{columnName}.Length!.Value";

                        if(column.IsNullable) {

                            addBeginAndEndLine = true;

                            string valueType = string.Empty;

                            if(dotNetType.IsValueType) {
                                valueType = ".Value";
                            }
                            if(isKeyColumn) {
                                valueType = ".Value.Value";
                            }
                            rule.Append($"{columnNameFirstLetterLowerCase}!{valueType}.Length >= 0 && {columnNameFirstLetterLowerCase}{valueType}.Length <= {maxLength})");
                            rule.EndLine().Indent(4);
                            rule.Append($".When(_x => _x.{columnName} != null)");
                        }
                        else {
                            string valueType = isKeyColumn ? ".Value" : "";
                            rule.Append($"{columnNameFirstLetterLowerCase}{valueType}.Length >= 0 && {columnNameFirstLetterLowerCase}{valueType}.Length <= {maxLength})");
                        }
                        rule.EndLine().Indent(4);
                        rule.Append($".WithMessage($\"{columnName} length must be between 0 and {{ {maxLength} }}\")");
                    }
                    hasRule = true;
                }

                rule.Append(";");

                if(hasRule) {
                    if(addBeginAndEndLine) {
                        validateCode.EndLine();
                    }
                    validateCode.Append(rule.ToString()).EndLine();
                    if(addBeginAndEndLine) {
                        validateCode.EndLine();
                    }
                }
            }

            validateCode.Indent(2).Append("}").EndLine();
            validateCode.Indent(1).Append("}").EndLine();

            classCode.Append(validateCode.ToString());

            classCode.Append("}");
            return classCode;
        }
    }
}