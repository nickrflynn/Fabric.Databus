﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabusSqlReader.cs" company="Health Catalyst">
//   
// </copyright>
// <summary>
//   Defines the IDatabusSqlReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Fabric.Databus.Interfaces.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Databus.Interfaces.Config;

    using Serilog;

    /// <summary>
    /// The DatabusSqlReader interface.
    /// </summary>
    public interface IDatabusSqlReader
    {
        /// <summary>
        /// The read data from query.
        /// </summary>
        /// <param name="load">
        /// The load.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="topLevelKeyColumn">
        /// The top Level Key Column.
        /// </param>
        /// <returns>
        /// The <see cref="ReadSqlDataResult"/>ReadSqlDataResult
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// exception thrown
        /// </exception>
        Task<ReadSqlDataResult> ReadDataFromQueryAsync(IDataSource load, string start, string end, ILogger logger, string topLevelKeyColumn);

        /// <summary>
        /// The get list of entity keys.
        /// </summary>
        /// <param name="topLevelKeyColumn">
        /// The top level key column.
        /// </param>
        /// <param name="maximumEntitiesToLoad">
        /// The maximum entities to load.
        /// </param>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        Task<IList<string>> GetListOfEntityKeysAsync(string topLevelKeyColumn, int maximumEntitiesToLoad, IDataSource dataSource);
    }
}