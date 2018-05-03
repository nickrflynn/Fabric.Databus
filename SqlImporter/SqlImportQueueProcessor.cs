﻿using ElasticSearchJsonWriter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ElasticSearchSqlFeeder.Interfaces;
using ElasticSearchSqlFeeder.Shared;
using Fabric.Databus.Config;
using ZipCodeToGeoCodeConverter;

namespace SqlImporter
{
    public class SqlImportQueueProcessor : BaseQueueProcessor<SqlImportQueueItem, ConvertDatabaseToJsonQueueItem>
    {
        public SqlImportQueueProcessor(QueueContext queueContext)
            : base(queueContext)
        {
        }

        private void ReadOneQueryFromDatabase(string queryId, DataSource load, int seed, string start, string end, int workitemBatchNumber)
        {
            try
            {
                InternalReadOneQueryFromDatabase(queryId, load, start, end, workitemBatchNumber);
            }
            catch (Exception e)
            {
                throw new Exception($"Connection String: {Config.ConnectionString}", e);
            }
        }

        private void InternalReadOneQueryFromDatabase(string queryId, DataSource load, string start, string end, int batchNumber)
        {
            var sqlJsonValueWriter = new SqlJsonValueWriter();

            var result = DatabusSqlReader.ReadDataFromQuery(Config, load, start, end, MyLogger);

            foreach (var frame in result.Data)
                {
                    AddToOutputQueue(new ConvertDatabaseToJsonQueueItem
                    {
                        BatchNumber = batchNumber,
                        QueryId = queryId,
                        JoinColumnValue = frame.Key,
                        Rows = frame.Value,
                        Columns = result.ColumnList,
                        PropertyName = load.Path,
                        PropertyType = load.PropertyType,
                        JsonValueWriter = sqlJsonValueWriter
                    });
                }

                //now all the source data has been loaded

                // handle fields without any transform
                var untransformedFields = load.Fields.Where(f => f.Transform == QueryFieldTransform.None)
                    .ToList();

                untransformedFields.ForEach(f => { });

                //EsJsonWriter.WriteRawDataToJson(data, columnList, seed, load.PropertyPath, 
                //    new SqlJsonValueWriter(), load.Index, load.EntityType);

                //esJsonWriter.WriteRawObjectsToJson(data, columnList, seed, load.PropertyPath, 
                //    new SqlJsonValueWriter(), load.Index, load.EntityType);

            MyLogger.Trace($"Finished reading rows for {queryId}");
        }


        protected override void Handle(SqlImportQueueItem workitem)
        {
            ReadOneQueryFromDatabase(workitem.QueryId, workitem.DataSource, workitem.Seed, workitem.Start, workitem.End, workitem.BatchNumber);
        }

        protected override void Begin(bool isFirstThreadForThisTask)
        {
        }

        protected override void Complete(string queryId, bool isLastThreadForThisTask)
        {
            //MarkOutputQueueAsCompleted();
        }

        protected override string GetId(SqlImportQueueItem workitem)
        {
            return workitem.QueryId;
        }

        protected override string LoggerName => "SqlImport";
    }

    public class SqlImportQueueItem : IQueueItem
    {
        public string PropertyName { get; set; }

        public string QueryId { get; set; }

        public DataSource DataSource { get; set; }

        public int Seed { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int BatchNumber { get; set; }
    }
}
