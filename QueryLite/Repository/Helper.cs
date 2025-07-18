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
using QueryLite.Databases;
using System.Data;
using System.Data.Common;

namespace QueryLite {

    public static class Helper { //TODO: Rename to something better

        

        //public static List<ColumnAndSetter<ROW>> GetWhereClauseColumnAndSetterList<ROW>(ATable table, List<ColumnAndSetter<ROW>> columnAndSetters, bool matchRowOnAllColumns) {

        //    if(matchRowOnAllColumns || table.PrimaryKey == null) {  //When there are no primary key columns the where clause needs to select on all columns
        //        return columnAndSetters;
        //    }
        //    Dictionary<string, ColumnAndSetter<ROW>> columnAndSettersLookup = columnAndSetters.ToDictionary(cs => cs.Column.ColumnName);
        //    return [.. table.PrimaryKey.Columns.Select(column => columnAndSettersLookup[column.ColumnName])];
        //}
    }
}