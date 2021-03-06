﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateBatchItemsPipelineStep.cs" company="Health Catalyst">
//   
// </copyright>
// <summary>
//   Defines the CreateBatchItemsPipelineStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CreateBatchItemsPipelineStep
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BasePipelineStep;

    using Fabric.Databus.Interfaces;
    using Fabric.Databus.Interfaces.Config;
    using Fabric.Databus.Interfaces.Loggers;
    using Fabric.Databus.Interfaces.Queues;

    using QueueItems;

    using Serilog;

    /// <inheritdoc />
    /// <summary>
    /// The create batch items queue processor.
    /// </summary>
    public class CreateBatchItemsPipelineStep : BasePipelineStep<IJsonObjectQueueItem, SaveBatchQueueItem>
    {
        /// <summary>
        /// The temporary cache.
        /// </summary>
        private readonly Queue<IJsonObjectQueueItem> temporaryCache = new Queue<IJsonObjectQueueItem>();

        /// <inheritdoc />
        public CreateBatchItemsPipelineStep(
            IJobConfig jobConfig, 
            ILogger logger, 
            IQueueManager queueManager, 
            IProgressMonitor progressMonitor, 
            CancellationToken cancellationToken) 
            : base(jobConfig, logger, queueManager, progressMonitor, cancellationToken)
        {
            if (this.Config.EntitiesPerUploadFile < 1)
            {
                throw new ArgumentException(nameof(this.Config.EntitiesPerUploadFile));
            }
        }

        /// <inheritdoc />
        protected override string LoggerName => "CreateBatchItems";

        /// <summary>
        /// The flush all documents.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task FlushAllDocuments()
        {
            while (this.temporaryCache.Any())
            {
                await this.FlushDocumentsToLimit(this.temporaryCache.Count);
            }
        }

        /// <inheritdoc />
        protected override async Task HandleAsync(IJsonObjectQueueItem workItem)
        {
            this.temporaryCache.Enqueue(workItem);
            await this.FlushDocumentsIfBatchSizeReachedAsync();
        }

        /// <inheritdoc />
        protected override async Task CompleteAsync(string queryId, bool isLastThreadForThisTask)
        {
            await this.FlushAllDocuments();
        }

        /// <inheritdoc />
        protected override string GetId(IJsonObjectQueueItem workItem)
        {
            return workItem.QueryId;
        }

        /// <summary>
        /// The flush documents if batch size reached.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task FlushDocumentsIfBatchSizeReachedAsync()
        {
            // see if there are enough to create a batch
            if (this.temporaryCache.Count > this.Config.EntitiesPerUploadFile)
            {
                await this.FlushDocumentsAsync();
            }
        }

        /// <summary>
        /// The flush documents.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task FlushDocumentsAsync()
        {
            await this.FlushDocumentsWithoutLockAsync();
        }

        /// <summary>
        /// The flush documents without lock.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task FlushDocumentsWithoutLockAsync()
        {
            while (this.temporaryCache.Count > this.Config.EntitiesPerUploadFile)
            {
                await this.FlushDocumentsToLimit(this.Config.EntitiesPerUploadFile);
            }
        }

        /// <summary>
        /// The flush documents to limit.
        /// </summary>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task FlushDocumentsToLimit(int limit)
        {
            var docsToSave = new List<IJsonObjectQueueItem>();
            for (int i = 0; i < limit; i++)
            {
                IJsonObjectQueueItem cacheItem = this.temporaryCache.Dequeue();
                docsToSave.Add(cacheItem);
            }

            if (docsToSave.Any())
            {
                this.MyLogger.Verbose($"Saved Batch: count:{docsToSave.Count} from {docsToSave.First().Id} to {docsToSave.Last().Id}, inQueue:{this.InQueue.Count} ");
                await this.AddToOutputQueueAsync(new SaveBatchQueueItem { ItemsToSave = docsToSave });
            }
        }
    }
}