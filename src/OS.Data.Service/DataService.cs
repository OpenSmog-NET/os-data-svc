using Marten;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OS.Data.v1;
using OS.Events.Data;
using Serilog.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OS.Data.Service
{
    public class DataService
    {
        private readonly ILogger<DataService> logger;
        private readonly IDocumentStore store;

        public DataService(ILogger<DataService> logger, IDocumentStore store)
        {
            this.logger = logger;
            this.store = store;
        }

        public async Task SaveMeasurements([QueueTrigger("measurements", Connection = "AzureStorage")] SaveMeasurementsCommand command)
        {
            try
            {
                using (LogContext.PushProperty("CorrelationId", command.CorrelationId))
                using (var session = store.OpenSession())
                {
                    var latestMeasurement = 0L;

                    if (session.Query<MeasurementData>().Any())
                    {
                        latestMeasurement = await session.Query<MeasurementData>()
                            .Where(x => x.DeviceId == command.DeviceId)
                            .MaxAsync(x => x.TimeStamp);
                    }

                    var notAdded = command.Measurements.Count();
                    command.Measurements.ToList().ForEach(x =>
                    {
                        if (x.Timestamp <= latestMeasurement) return;

                        session.Store(MeasurementDataMapper.MapToDAL(command.DeviceId, x));
                        notAdded--;
                    });

                    await session.SaveChangesAsync();

                    logger.LogInformation("{@DeviceId} Added {@Added} measurements. Skipped {@Skipped} measurements",
                        command.DeviceId, command.Measurements.Count() - notAdded, notAdded);
                }
            }
            catch (Exception ex)
            {
            }
        }

        [NoAutomaticTrigger]
        public async Task RunAsync()
        {
            logger.LogInformation("WebJob is starting");

            var token = new WebJobsShutdownWatcher().Token;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), token);
            }

            logger.LogInformation("WebJob is shutting down");
        }
    }
}