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
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// This attribute stops an error appearing in the schema validator when this column type does not map correctly to the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SuppressColumnTypeValidationAttribute : Attribute {

    }

    /// <summary>
    /// Interface that can be used for Columns that have an unsupported type. e.g. public Column<IUnsupportedType> ColumnName { get; }
    /// </summary>
    public interface IUnsupportedType { }

    /// <summary>
    /// This is a place older interface to indicate that the Sql Server Geography type is unsupported by Query Lite.
    /// </summary>
    public interface IGeography_UnsupportedType : IUnsupportedType { }

    public sealed class PrimaryKey {

        public PrimaryKey(ITable table, string constraintName, params IColumn[] columns) {

            ArgumentException.ThrowIfNullOrEmpty(constraintName);

            if(columns.Length == 0) {
                throw new ArgumentException($"{nameof(columns)} must contain at least one column");
            }

            foreach(IColumn column in columns) {

                if(column.Table != table) {
                    throw new Exception($"{nameof(column)} parent table must be the same table object as the {nameof(table)} parameter");
                }
            }
            Table = table;
            ConstraintName = constraintName;
            Columns = columns;
        }
        public ITable Table { get; }
        public string ConstraintName { get; }
        public IColumn[] Columns { get; }
    }

    public sealed class ForeignKey {

        public ForeignKey(ITable table, string constraintName) {

            ArgumentException.ThrowIfNullOrEmpty(constraintName);
            Table = table;
            ConstraintName = constraintName;
        }
        public ITable Table { get; }
        public string ConstraintName { get; }
        public List<ForeignKeyReference> ColumnReferences { get; } = new List<ForeignKeyReference>();

        public ForeignKey References<TYPE>(AColumn<TYPE> foreignKeyColumn, AColumn<TYPE> primaryKeyColumn) where TYPE : notnull {
            ColumnReferences.Add(new ForeignKeyReference(foreignKeyColumn, primaryKeyColumn));
            return this;
        }
        public ForeignKey ReferencesNonMatching<TYPE_A, TYPE_B>(AColumn<TYPE_A> foreignKeyColumn, AColumn<TYPE_B> primaryKeyColumn) where TYPE_A : notnull where TYPE_B : notnull {
            ColumnReferences.Add(new ForeignKeyReference(foreignKeyColumn, primaryKeyColumn));
            return this;
        }
    }

    public sealed class ForeignKeyReference {

        public ForeignKeyReference(IColumn foreignKeyColumn, IColumn primaryKeyColumn) {
            ForeignKeyColumn = foreignKeyColumn;
            PrimaryKeyColumn = primaryKeyColumn;
        }
        public IColumn ForeignKeyColumn { get; }
        public IColumn PrimaryKeyColumn { get; }
    }

    public sealed class UniqueConstraint {

        public UniqueConstraint(ITable table, string constraintName, params IColumn[] columns) {

            ArgumentException.ThrowIfNullOrEmpty(constraintName);

            if(columns.Length == 0) {
                throw new ArgumentException($"{nameof(columns)} must contain at least one column");
            }

            foreach(IColumn column in columns) {

                if(column.Table != table) {
                    throw new Exception($"{nameof(column)} parent table must be the same table object as the {nameof(table)} parameter");
                }
            }
            Table = table;
            ConstraintName = constraintName;
            Columns = columns;
        }
        public ITable Table { get; }
        public string ConstraintName { get; }
        public IColumn[] Columns { get; }
    }
}