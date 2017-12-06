using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using OS.Events;
using OS.Events.Data;
using System;

namespace OS.Data.Service
{
    public class TelemetryService
    {
        private readonly TelemetryClient client;

        public TelemetryService(TelemetryClient client)
        {
            this.client = client;
        }

        public void TrackCommand(SaveMeasurementsCommand command, DateTime arrivalDate)
        {
            TrackDomainEvent(command, arrivalDate);
        }

        private void TrackDomainEvent(IDomainEvent @event, DateTime arrivalDate)
        {
            client.TrackEvent(@event.GetType().Name);
            client.TrackMetric(new MetricTelemetry($"{@event.GetType().Name}_TimeInTransport", (arrivalDate - @event.CreatedDate).TotalMilliseconds));
        }
    }
}