﻿using System;
using System.Linq;

using Fabric.Databus.API.Configuration;
using Fabric.Databus.Domain.Jobs;
using Nancy;
using Nancy.Security;
using Serilog;

namespace Fabric.Databus.API.Modules
{
    using Fabric.Databus.Shared;

    public class JobStatusModule : NancyModule
		{
				public JobStatusModule(ILogger logger, IJobScheduler jobScheduler, IAppConfiguration configuration) : base("/jobstatus")
				{
				    this.RequiresClaimsIfAuthorizationEnabled(configuration, claim => claim.Value.Equals("fabric/databus.queuejob", StringComparison.OrdinalIgnoreCase));

						Get("/", parameters =>
						{
						    JobHistoryItem lastjob = jobScheduler.GetMostRecentJobs(1).FirstOrDefault();

						    return Negotiate
						        .WithModel(lastjob)
						        .WithView("ShowJobStatus");
                        });

						Get("/{jobName}", parameters =>
						{
								var jobName = parameters.jobName;
								if (Guid.TryParse(jobName, out Guid jobGuid))
								{
										JobHistoryItem model = jobScheduler.GetJobStatus(jobGuid);

										return Negotiate
												.WithModel(model)
												.WithView("ShowJobStatus");
								}

								return jobScheduler.GetJobHistory(parameters.jobName);
						});
				}
		}
}
