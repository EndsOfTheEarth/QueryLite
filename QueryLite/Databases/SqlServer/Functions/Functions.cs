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

namespace QueryLite.Databases.SqlServer.Functions {

    /// <summary>
    /// Sql Count(*) function
    /// </summary>
    public sealed class COUNT_ALL : Function<int> {

        public static COUNT_ALL Instance { get; } = new COUNT_ALL();

        private COUNT_ALL() : base("COUNT(*)") { }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return "COUNT(*)";
        }
    }

    /// <summary>
    /// Sql GETDATE() function
    /// </summary>
    public sealed class GETDATE : Function<DateTime> {

        public static GETDATE Instance { get; } = new GETDATE();

        private GETDATE() : base("GETDATE()") { }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return "GETDATE()";
        }
    }

    /// <summary>
    /// Sql NEWID() function
    /// </summary>
    public sealed class NEWID : Function<Guid> {

        public static NEWID Instance { get; } = new NEWID();

        private NEWID() : base("NEWID()") { }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return "NEWID()";
        }
    }

    /// <summary>
    /// Sql SYSDATETIMEOFFSET() function
    /// </summary>
    public sealed class SYSDATETIMEOFFSET : Function<DateTimeOffset> {

        public static SYSDATETIMEOFFSET Instance { get; } = new SYSDATETIMEOFFSET();

        private SYSDATETIMEOFFSET() : base("SYSDATETIMEOFFSET()") { }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return "SYSDATETIMEOFFSET()";
        }
    }
}