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
using System.Data.Common;

namespace QueryLite {

    /// <summary>
    /// The parameters class is used to help sql query generators create and execute queries with parameters
    /// </summary>
    public interface IParameters {

        /// <summary>
        /// Add a new parameter to query
        /// </summary>
        /// <param name="database">Database being used for sql query</param>
        /// <param name="type">.net type of parameter value</param>
        /// <param name="value">Value of parameter</param>
        /// <param name="paramName">Name of the newly generated parameter name for the sql query</param>
        void Add(IDatabase database, Type type, object? value, out string paramName);

        /// <summary>
        /// Set parameters on the sql command
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        void SetParameters(IDatabase database, DbCommand command);
    }

    public enum Parameters {

        /// <summary>
        /// Use the default parameters setting. This default value is located in Settings.UseParameters
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use parameters in sql query
        /// </summary>
        On = 1,

        /// <summary>
        /// Do not use parameters in sql query
        /// </summary>
        Off = 2
    }
}