/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class TableCodeGenerator {

        public static CodeBuilder Generate(List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.Append("using System;").EndLine();
            code.Append("using QueryLite;").EndLine();

            bool skipInterfaces = tables.Count == 1 && tables[0].IsView;

            List<SchemaName> schemaNames = [];

            foreach(DatabaseTable table in tables) {

                if(!schemaNames.Contains(table.Schema)) {
                    schemaNames.Add(table.Schema);
                }
            }

            schemaNames.Sort((a, b) => a.Value.CompareTo(b.Value));

            int count = 0;

            foreach(SchemaName schema in schemaNames) {

                if(count > 0) {
                    code.EndLine();
                }
                count++;
                code.EndLine().Append($"namespace {settings.Namespaces.GetTableNamespace(schema)} {{").EndLine();

                foreach(DatabaseTable table in tables) {

                    if(string.Equals(table.Schema.Value, schema.Value, StringComparison.OrdinalIgnoreCase)) {
                        TablePrefix prefix = new TablePrefix(table);
                        code.Append(Generate(table, prefix, settings, includeUsings: false).ToString());
                    }
                }
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder Generate(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings) {

            CodeBuilder code = new CodeBuilder();

            if(includeUsings) {

                code.Append($"namespace {settings.Namespaces.GetTableNamespace(table.Schema)} {{").EndLine().EndLine();

                code.Indent(1).Append("using System;").EndLine();
                code.Indent(1).Append("using QueryLite;").EndLine();
            }

            code.EndLine();

            string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

            ClassDeclarationSyntax classDeclaration = GenerateClass(
                instanceNumber: settings.NumberOfInstanceProperties,
                table, settings
            );

            string c = classDeclaration.NormalizeWhitespace().ToFullString();

            code.Indent(1).Append($"public sealed class {tableClassName} : ATable {{").EndLine().EndLine();

            code.Indent(2).Append($"public static readonly {tableClassName} Instance = new();").EndLine();

            for(int index = 1; index < settings.NumberOfInstanceProperties; index++) {
                code.Indent(2).Append($"public static readonly {tableClassName} Instance{index + 1} = new();").EndLine();
            }

            List<string> lines = [];

            code.EndLine();

            int count = 0;

            foreach(DatabaseColumn column in table.Columns) {

                CodeHelper.ColumnInfo columnInfo = CodeHelper.GetColumnInfo(table, column, useIdentifiers: settings.UseIdentifiers);

                string columnClass = !column.IsNullable ? "Column" : "NColumn";

                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                bool addSuppressAttribute = false;

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IUnsupportedType))) {    //Ignore unsupported types
                    addSuppressAttribute = true;
                    code.EndLine();
                    code.Indent(2).Append("[SuppressColumnTypeValidation] --> ***PLEASE_CHECK_UNSUPPORTED_TYPE***").EndLine();
                }
                count++;

                string underlyingTypeText = "";

                if(columnInfo.IdentifierType == IdentifierType.Custom) {
                    underlyingTypeText = $", {columnInfo.UnderlyingTypeName}";
                }

                code.Indent(2).Append($"public {columnClass}<{columnInfo.ColumnTypeName}{underlyingTypeText}> {columnName} {{ get; }}").EndLine();

                if(addSuppressAttribute) {
                    code.EndLine();
                }

                string columnLengthParameter = "";

                if(column.Length?.LengthType == LengthType.Max || columnInfo.DotNetType == typeof(byte[])) {
                    columnLengthParameter = $", length: {nameof(ColumnLength)}.MAX";
                }
                else if(column.Length != null) {
                    columnLengthParameter = $", length: new({column.Length?.Length})";
                }

                string encloseParameter = SqlKeyWordLookup.IsKeyWord(column.ColumnName.Value) ? ", enclose: true" : "";

                string columnDescription = settings.IncludeDescriptions ? $", desc: \"{CodeHelper.EscapeCSharpString(column.Description)}\"" : "";

                lines.Add($"{columnName} = new {columnClass}<{columnInfo.ColumnTypeName}{underlyingTypeText}>(this, name: \"{column.ColumnName.Value}\"{(column.IsAutoGenerated ? ", isAutoGenerated: true" : "")}{columnLengthParameter}{encloseParameter}{columnDescription});");
            }

            if(table.PrimaryKey != null && settings.IncludeConstraints) {

                code.EndLine();
                code.Indent(2).Append($"public override PrimaryKey? PrimaryKey => new(table: this, name: \"{table.PrimaryKey.ConstraintName}\"");

                foreach(string columnName in table.PrimaryKey.ColumnNames) {

                    foreach(DatabaseColumn column in table.Columns) {

                        if(string.Compare(columnName, column.ColumnName.Value, ignoreCase: true) == 0) {
                            code.Append(", ").Append(prefix.GetColumnName(column.ColumnName.Value, className: tableClassName));
                            break;
                        }
                    }
                }
                code.Append(");").EndLine();
            }

            if(table.UniqueConstraints.Count > 0 && settings.IncludeConstraints) {

                code.EndLine();

                code.Indent(2).Append("public override UniqueConstraint[] UniqueConstraints => [").EndLine();

                for(int index = 0; index < table.UniqueConstraints.Count; index++) {

                    DatabaseUniqueConstraint uniqueConstraint = table.UniqueConstraints[index];

                    if(index > 0) {
                        code.Append(",").EndLine();
                    }
                    code.Indent(3).Append($"new(this, name: \"{uniqueConstraint.ConstraintName}\"");

                    foreach(ColumnName columnName in uniqueConstraint.ColumnNames) {
                        code.Append(", ").Append(prefix.GetColumnName(columnName.Value, className: tableClassName));
                    }
                    code.Append(")");
                }
                code.EndLine();
                code.Indent(2).Append("];").EndLine();
            }

            if(table.ForeignKeys.Count > 0 && settings.IncludeConstraints) {

                code.EndLine();

                code.Indent(2).Append("public override ForeignKey[] ForeignKeys => [").EndLine();

                for(int index = 0; index < table.ForeignKeys.Count; index++) {

                    DatabaseForeignKey foreignKey = table.ForeignKeys[index];

                    if(index > 0) {
                        code.Append(",").EndLine();
                    }
                    code.Indent(3).Append($"new ForeignKey(this, name: \"{foreignKey.ConstraintName}\")");

                    foreach(DatabaseForeignKeyReference reference in foreignKey.References) {

                        string foreignKeyColumnName = prefix.GetColumnName(reference.ForeignKeyColumn.ColumnName.Value, className: tableClassName);

                        TablePrefix primaryKeyTablePrefix = new TablePrefix(reference.PrimaryKeyColumn.Table);

                        string primaryKeyTable = CodeHelper.GetTableName(reference.PrimaryKeyColumn.Table, includePostFix: true);
                        string primaryKeyColumnName = primaryKeyTablePrefix.GetColumnName(reference.PrimaryKeyColumn.ColumnName.Value, className: null);

                        string primaryKeyTableSchemaName = !Namespaces.IsDefaultSchema(reference.PrimaryKeyColumn.Table.Schema) ? $"{reference.PrimaryKeyColumn.Table.Schema.Value}." : "";

                        code.Append($".References({foreignKeyColumnName}, {primaryKeyTableSchemaName}{primaryKeyTable}.Instance.{primaryKeyColumnName})");
                    }
                }
                code.EndLine();
                code.Indent(2).Append("];").EndLine();
            }

            if(table.CheckConstraints.Count > 0 && settings.IncludeConstraints) {

                code.EndLine();

                code.Indent(2).Append("public override CheckConstraint[] CheckConstraints => [").EndLine();

                for(int index = 0; index < table.CheckConstraints.Count; index++) {

                    DatabaseCheckConstraint checkConstraint = table.CheckConstraints[index];

                    if(index > 0) {
                        code.Append(",").EndLine();
                    }
                    code.Indent(3).Append($"new(name: \"{checkConstraint.ConstraintName}\")");
                }
                code.EndLine();
                code.Indent(2).Append("];").EndLine();
            }

            code.EndLine();

            string encloseTableName = SqlKeyWordLookup.IsKeyWord(table.TableName.Value) ? ", enclose: true" : "";

            string isViewCode = table.IsView ? ", isView: true" : "";

            string tableDescription = settings.IncludeDescriptions ? $", desc: \"{CodeHelper.EscapeCSharpString(table.Description)}\"" : "";

            code.Indent(2).Append($"private {tableClassName}() : base(name: \"{table.TableName.Value}\", schemaName: \"{table.Schema}\"{encloseTableName}{isViewCode}{tableDescription}) {{").EndLine().EndLine();

            foreach(string line in lines) {
                code.Indent(3).Append(line).EndLine();
            }
            code.Indent(2).Append("}").EndLine();
            code.Indent(1).Append("}").EndLine();

            if(settings.UseRepositoryPattern) {

                string tableName = CodeHelper.GetTableName(table, includePostFix: false);

                code.EndLine();
                code.Indent(1).Append(@$"[Repository<{tableClassName}>(MatchOn.AllColumns, repositoryName: ""{tableName}Repository"")]").EndLine();
                code.Indent(1).Append($"public partial record {tableName}Row {{").EndLine();
                code.EndLine();
                code.Indent(1).Append("}").EndLine();
            }

            if(includeUsings) {
                code.Append("}");
            }
            return code;
        }

        private static ClassDeclarationSyntax GenerateClass(int instanceNumber, DatabaseTable table, CodeGeneratorSettings settings) {

            string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

            TablePrefix prefix = new(table);
            // : ATable
            ClassDeclarationSyntax classDeclaration =
                ClassDeclaration(tableClassName)
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword))
                .AddBaseListTypes(SimpleBaseType(ParseName("ATable")))
                .AddMembers(
                    GenerateInstanceProperties(type: tableClassName, number: instanceNumber)
                )
                .AddMembers(
                    GenerateColumnProperties(table, tableClassName: tableClassName, prefix, settings)
                );

            if(table.PrimaryKey != null) {
                classDeclaration = classDeclaration.AddMembers(
                    GeneratePrimaryKeyProperty(table.PrimaryKey)
                    );
            }

            if(table.UniqueConstraints.Count > 0 || !settings.IncludeConstraints) {
                classDeclaration = classDeclaration.AddMembers(
                    GenerateUniqueConstraints(table.UniqueConstraints, tableClassName, prefix)
                );
            }

            if(table.ForeignKeys.Count > 0 || !settings.IncludeConstraints) {
                classDeclaration = classDeclaration.AddMembers(
                    GenerateForeignKeyConstraints(table.ForeignKeys, prefix)
                );                
            }

            return classDeclaration;
        }

        private static PropertyDeclarationSyntax[] GenerateInstanceProperties(string type, int number) {

            List<PropertyDeclarationSyntax> list = [];

            for(int index = 0; index < number; index++) {

                string propertyName = "Instance";

                if(index != 0 || number != 1) {
                    propertyName += index;
                }
                list.Add(GenerateInstanceProperty(propertyName, type));
            }
            return [.. list];
        }
        private static PropertyDeclarationSyntax GenerateInstanceProperty(string propertyName, string type) {

            /*
             * 
             * This generates the table instance property.
             * 
             * e.g. public static readonly tableClassName Instance { get; } = new();
             * 
             */
            TypeSyntax propertyType = ParseTypeName(type);

            AccessorDeclarationSyntax getAccessor =
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            //AccessorDeclarationSyntax setAccessor = SyntaxFactory
            //    .AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            //    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            AccessorListSyntax accessorList = AccessorList(
                List([getAccessor])   //, setAccessor
            );

            PropertyDeclarationSyntax propertyDeclaration =
                PropertyDeclaration(propertyType, propertyName)
                .AddModifiers(  //public static readonly
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword)
                 )
                .WithAccessorList(accessorList)
                .WithInitializer(
                    EqualsValueClause(        // = new();
                        ImplicitObjectCreationExpression(
                                Token(SyntaxKind.NewKeyword),
                                argumentList: ArgumentList(),
                                initializer: null
                            )
                        )
                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return propertyDeclaration;
        }

        private static PropertyDeclarationSyntax[] GenerateColumnProperties(DatabaseTable table, string tableClassName, TablePrefix prefix, CodeGeneratorSettings settings) {

            List<PropertyDeclarationSyntax> list = [];

            foreach(DatabaseColumn column in table.Columns) {

                CodeHelper.ColumnInfo columnInfo = CodeHelper.GetColumnInfo(table, column, useIdentifiers: settings.UseIdentifiers);

                string columnClass = !column.IsNullable ? "Column" : "NColumn";

                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                AttributeSyntax? suppressColumnTypeValidationAttribute = null;

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IUnsupportedType))) {    //Ignore unsupported types
                    //addSuppressAttribute = true;
                    //code.EndLine();
                    //code.Indent(2).Append("[SuppressColumnTypeValidation] --> ***PLEASE_CHECK_UNSUPPORTED_TYPE***").EndLine();
                    suppressColumnTypeValidationAttribute = Attribute(IdentifierName("SuppressColumnTypeValidation"));
                }

                //AttributeListSyntax attributeList = AttributeList(asd
                //    attribute != null ? SingletonSeparatedList(attribute) : []
                //);

                TypeSyntax[] genericArgs;

                if(columnInfo.IdentifierType == IdentifierType.Custom) {

                    genericArgs = [ //e.g.  <PersonId, int>
                        IdentifierName(columnInfo.ColumnTypeName),
                        IdentifierName(columnInfo.UnderlyingTypeName)
                    ];
                }
                else {
                    genericArgs = [IdentifierName(columnInfo.ColumnTypeName)];
                }

                PropertyDeclarationSyntax property = PropertyDeclaration(
                    // Type: Column<TYPE, UNDERLYING_TYPE>
                    GenericName(Identifier(columnClass))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SeparatedList(genericArgs)
                            )
                        ),
                    Identifier(columnName)
                )

                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword))
                )
                // Accessor List: { get; }
                .WithAccessorList(
                    AccessorList(
                        SingletonList(
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        )
                    )
                );

                if(suppressColumnTypeValidationAttribute != null) {

                    //e.g. [SuppressColumnTypeValidation] attibute on column property
                    property = property.WithAttributeLists(
                        SingletonList(
                            AttributeList(
                                SingletonSeparatedList(suppressColumnTypeValidationAttribute)
                            )
                        )
                    );
                }

                property = property.WithTrailingTrivia(CarriageReturnLineFeed);

                list.Add(property);

                /*
                
                    bool addSuppressAttribute = false;

                    if(column.DataType.DotNetType.IsAssignableTo(typeof(IUnsupportedType))) {    //Ignore unsupported types
                        addSuppressAttribute = true;
                        code.EndLine();
                        code.Indent(2).Append("[SuppressColumnTypeValidation] --> ***PLEASE_CHECK_UNSUPPORTED_TYPE***").EndLine();
                    }
                    count++;

                    string underlyingTypeText = "";

                    if(columnInfo.IdentifierType == IdentifierType.Custom) {
                        underlyingTypeText = $", {columnInfo.UnderlyingTypeName}";
                    }

                    code.Indent(2).Append($"public {columnClass}<{columnInfo.ColumnTypeName}{underlyingTypeText}> {columnName} {{ get; }}").EndLine();

                    if(addSuppressAttribute) {
                        code.EndLine();
                    }

                    string columnLengthParameter = "";

                    if(column.Length?.LengthType == LengthType.Max || columnInfo.DotNetType == typeof(byte[])) {
                        columnLengthParameter = $", length: {nameof(ColumnLength)}.MAX";
                    }
                    else if(column.Length != null) {
                        columnLengthParameter = $", length: new({column.Length?.Length})";
                    }

                    string encloseParameter = SqlKeyWordLookup.IsKeyWord(column.ColumnName.Value) ? ", enclose: true" : "";

                    string columnDescription = settings.IncludeDescriptions ? $", desc: \"{CodeHelper.EscapeCSharpString(column.Description)}\"" : "";

                    lines.Add($"{columnName} = new {columnClass}<{columnInfo.ColumnTypeName}{underlyingTypeText}>(this, name: \"{column.ColumnName.Value}\"{(column.IsAutoGenerated ? ", isAutoGenerated: true" : "")}{columnLengthParameter}{encloseParameter}{columnDescription});");
                */
            }
            return [.. list];
        }

        /*
         * Generates Primary Key property.
         * 
         * e.g.  public override PrimaryKey? PrimaryKey => new(table: this, name: "pk_name", Id);
         */
        private static PropertyDeclarationSyntax GeneratePrimaryKeyProperty(DatabasePrimaryKey primaryKey) {

            List<SyntaxNodeOrToken> pkArgumentList = [
                //e.g. (table: this
                Argument(ThisExpression()).WithNameColon(NameColon(IdentifierName("table"))),
                //e.g. (table: this,
                Token(SyntaxKind.CommaToken),
                Argument(
                    //e.g. name: "pk_name"
                    LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        Literal(primaryKey.ConstraintName)
                    )
                ).WithNameColon(NameColon(IdentifierName("name")))
            ];

            foreach(string columnName in primaryKey.ColumnNames) {  //Add columns to pk arguments
                pkArgumentList.Add(Token(SyntaxKind.CommaToken));
                pkArgumentList.Add(Argument(IdentifierName(columnName)));
            }

            PropertyDeclarationSyntax property = PropertyDeclaration(
                    IdentifierName("PrimaryKey?"),
                    Identifier("PrimaryKey")
                )
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword)
                    )
                )
                .WithExpressionBody(
                    ArrowExpressionClause(
                        ImplicitObjectCreationExpression(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    pkArgumentList
                                )
                            ),
                            initializer: null
                        )
                    )
                )
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken)
                )
                .WithTrailingTrivia(CarriageReturnLineFeed);

            return property;
        }

        /*
         *
         *  Create unique constraints property.
         *
         *  e.g.
         *  public override UniqueConstraint[] UniqueConstraints => [
         *      new(this, name: "unq_table_name", Name),
         *      new(this, name: "unq_table_name2", Name)
         *  ];
         */
        private static PropertyDeclarationSyntax GenerateUniqueConstraints(List<DatabaseUniqueConstraint> constraints,
                                                                           string tableClassName,
                                                                           TablePrefix prefix) {

            List<CollectionElementSyntax> arrayElements = [];

            foreach(DatabaseUniqueConstraint constraint in constraints) {

                arrayElements.Add(
                    ExpressionElement(
                        GenerateConstraintCreation(constraint, tableClassName, prefix)
                    )
                );
            }

            ArrayTypeSyntax returnType = ArrayType( //e.g. UniqueConstraint[]
                IdentifierName(nameof(UniqueConstraint)))
                .WithRankSpecifiers(
                    SingletonList(
                        ArrayRankSpecifier(
                            SingletonSeparatedList<ExpressionSyntax>(
                                OmittedArraySizeExpression()
                            )
                        )
                    )
                );

            PropertyDeclarationSyntax property =
                PropertyDeclaration(returnType, Identifier("UniqueConstraints"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        CollectionExpression(SeparatedList(arrayElements))
                    )
                )
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return property;
        }

        /*
         * This generates a unique constraint creation syntax.
         * 
         * e.g. new(this, name: "constraint_name", NameA, NameB)
         * 
         * Which is part of:
         * 
         * public override UniqueConstraint[] UniqueConstraints => [
         *     new(this, name: "constraint_name", NameA, NameB)
         * ];
         */
        private static ImplicitObjectCreationExpressionSyntax GenerateConstraintCreation(
                                                        DatabaseUniqueConstraint constraint,
                                                        string tableClassName,
                                                        TablePrefix prefix) {

            List<ArgumentSyntax> arguments = [
                //(this
                Argument(ThisExpression()),
                //(this, name: "constraint_name"
                Argument(LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(constraint.ConstraintName)
                )).WithNameColon(NameColon(IdentifierName("name")))
            ];

            foreach(ColumnName columnName in constraint.ColumnNames) {
                string column = prefix.GetColumnName(columnName.Value, className: tableClassName);
                arguments.Add(Argument(IdentifierName(column)));
            }

            ImplicitObjectCreationExpressionSyntax expression = ImplicitObjectCreationExpression(
                newKeyword: Token(SyntaxKind.NewKeyword),
                argumentList: ArgumentList(SeparatedList(arguments)),
                initializer: null
            );
            return expression;
        }

        /*
         *
         *  Create foreign keys constraint property.
         *
         *  e.g.
         *  public override UniqueConstraint[] UniqueConstraints => [
         *      new(this, name: "unq_table_name", Name),
         *      new(this, name: "unq_table_name2", Name)
         *  ];
         */
        private static PropertyDeclarationSyntax GenerateForeignKeyConstraints(List<DatabaseForeignKey> constraints,
                                                                               TablePrefix prefix) {

            List<CollectionElementSyntax> arrayElements = [];

            foreach(DatabaseForeignKey constraint in constraints) {

                arrayElements.Add(
                    ExpressionElement(
                        GenerateForeignKeyCreation(constraint, prefix)
                    )
                );
            }

            ArrayTypeSyntax returnType = ArrayType( //e.g. ForeignKey[]
                IdentifierName(nameof(ForeignKey)))
                .WithRankSpecifiers(
                    SingletonList(
                        ArrayRankSpecifier(
                            SingletonSeparatedList<ExpressionSyntax>(
                                OmittedArraySizeExpression()
                            )
                        )
                    )
                );

            PropertyDeclarationSyntax property =
                PropertyDeclaration(returnType, Identifier("ForeignKeys"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        CollectionExpression(SeparatedList(arrayElements))
                    )
                )
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return property;
        }

        /*
         * 
         * Create foreign key creation syntax.
         * e.g.
         * new ForeignKey(this, name: "fk_Milestone_Tracker").References(TrackerId, TrackerTable.Instance.Id)
         */
        private static ExpressionSyntax GenerateForeignKeyCreation(DatabaseForeignKey foreignKey, TablePrefix prefix) {

            ExpressionSyntax expression = ObjectCreationExpression(
                IdentifierName(nameof(ForeignKey))
            )
            .WithArgumentList(
                ArgumentList(
                    SeparatedList([
                        Argument(ThisExpression()),
                        Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(foreignKey.ConstraintName))
                        ).WithNameColon(NameColon(IdentifierName("name")))
                    ])
                )
            );

            foreach(DatabaseForeignKeyReference reference in foreignKey.References) {

                string primaryKeyTableClassName = CodeHelper.GetTableName(
                    table: reference.PrimaryKeyColumn.Table,
                    includePostFix: true
                );

                TablePrefix primaryKeyTablePrefix = new TablePrefix(reference.PrimaryKeyColumn.Table);

                string primaryKeyColumnName = primaryKeyTablePrefix.GetColumnName(
                    reference.PrimaryKeyColumn.ColumnName.Value,
                    className: primaryKeyTableClassName
                );

                string foreignKeyTableClassName = CodeHelper.GetTableName(
                    table: reference.ForeignKeyColumn.Table,
                    includePostFix: true
                );

                string foreignKeyColumnName = prefix.GetColumnName(
                    reference.ForeignKeyColumn.ColumnName.Value,
                    className: foreignKeyTableClassName
                );

                expression =
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression,
                            IdentifierName("References")
                        )
                    )
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList([
                                Argument(IdentifierName(foreignKeyColumnName)),
                                Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(primaryKeyTableClassName),
                                            IdentifierName("Instance")
                                        ),
                                        IdentifierName(primaryKeyColumnName)
                                    )
                                )
                            ])
                        )
                    );
            }
            return expression;
        }
    }
}