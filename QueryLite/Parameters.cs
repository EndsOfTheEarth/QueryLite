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