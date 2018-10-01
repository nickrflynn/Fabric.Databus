﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElasticSearchJsonResponseItem.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ElasticSearchJsonResponseItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ElasticSearchApiCaller
{
    /// <summary>
    /// The elastic search json response item.
    /// </summary>
    public class ElasticSearchJsonResponseItem
    {
        /// <summary>
        /// Gets or sets the update.
        /// </summary>
        // ReSharper disable once StyleCop.SA1300
        // ReSharper disable once InconsistentNaming
        public ElasticSearchJsonResponseUpdate update { get; set; }
    }
}