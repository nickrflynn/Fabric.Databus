﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadSqlDataResult.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ReadSqlDataResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ElasticSearchSqlFeeder.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The read sql data result.
    /// </summary>
    public class ReadSqlDataResult
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public Dictionary<string, List<object[]>> Data { get; set; }

        /// <summary>
        /// Gets or sets the column list.
        /// </summary>
        public List<ColumnInfo> ColumnList { get; set; }
    }
}
