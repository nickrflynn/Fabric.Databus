﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploadPipelineStep.cs" company="Health Catalyst">
//   
// </copyright>
// <summary>
//   Defines the FileUploadPipelineStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploadPipelineStep
{
    using System.Threading;

    using BasePipelineStep;

    using Fabric.Databus.Interfaces;
    using Fabric.Databus.Interfaces.Config;
    using Fabric.Databus.Interfaces.ElasticSearch;
    using Fabric.Databus.Interfaces.Loggers;
    using Fabric.Databus.Interfaces.Queues;

    using QueueItems;

    using Serilog;

    /// <inheritdoc />
    /// <summary>
    /// The file upload queue processor.
    /// </summary>
    public class FileUploadPipelineStep : BasePipelineStep<FileUploadQueueItem, EndPointQueueItem>
    {
        /// <summary>
        /// The file uploader.
        /// </summary>
        private readonly IElasticSearchUploader elasticSearchUploader;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FileUploadPipelineStep.FileUploadPipelineStep" /> class.
        /// </summary>
        /// <param name="jobConfig">
        /// The queue context.
        /// </param>
        /// <param name="elasticSearchUploader">
        /// The elasticSearchUploader
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueManager">
        /// The queue Manager.
        /// </param>
        /// <param name="progressMonitor"></param>
        /// <param name="cancellationToken"></param>
        public FileUploadPipelineStep(
            IJobConfig jobConfig, 
            IElasticSearchUploader elasticSearchUploader, 
            ILogger logger, 
            IQueueManager queueManager, 
            IProgressMonitor progressMonitor,
            CancellationToken cancellationToken)
            : base(jobConfig, logger, queueManager, progressMonitor, cancellationToken)
        {
            this.elasticSearchUploader = elasticSearchUploader;
        }

        /// <summary>
        /// The logger name.
        /// </summary>
        protected override string LoggerName => "FileUpload";

        /// <inheritdoc />
        protected override async System.Threading.Tasks.Task HandleAsync(FileUploadQueueItem workItem)
        {
            await this.UploadFileAsync(workItem);
        }

        /// <inheritdoc />
        protected override async System.Threading.Tasks.Task BeginAsync(bool isFirstThreadForThisTask)
        {
            if (isFirstThreadForThisTask)
            {
                await this.elasticSearchUploader.StartUploadAsync();
            }
        }

        /// <inheritdoc />
        protected override async System.Threading.Tasks.Task CompleteAsync(string queryId, bool isLastThreadForThisTask)
        {
            if (isLastThreadForThisTask)
            {
                await this.elasticSearchUploader.FinishUploadAsync();
            }
        }

        /// <summary>
        /// The get id.
        /// </summary>
        /// <param name="workItem">
        /// The work item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetId(FileUploadQueueItem workItem)
        {
            return workItem.QueryId;
        }

        /// <summary>
        /// The upload file.
        /// </summary>
        /// <param name="wt">
        /// The wt.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async System.Threading.Tasks.Task UploadFileAsync(FileUploadQueueItem wt)
        {
            await this.elasticSearchUploader.SendDataToHostsAsync(
                    wt.BatchNumber,
                    wt.Stream,
                    doLogContent: false,
                    doCompress: this.Config.CompressFiles);

            this.MyLogger.Verbose($"Uploaded batch: {wt.BatchNumber} ");

        }
    }
}
