using Google.Api;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;

namespace WebApplication1.Repositories
{
    public class LogsRepository
    {
        private string _projectId;
        public LogsRepository(string projectId)
        {
            _projectId = projectId;
        }

        public void WriteLogEntry(string logId, string message, LogSeverity severity = LogSeverity.Info)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(_projectId, logId);
            var jsonPayload = new Struct()
            {
                Fields =
                    {
                        { "message", Value.ForString(message) }
                    }
            };
            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = severity,
                JsonPayload = jsonPayload
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };

            client.WriteLogEntries(logName, resource, null,
                new[] { logEntry }, null);

        }
    }
}
