namespace RetailProject.Models
{
    // Represents a log entry stored in Azure Files
    public class LogEntry
    {
        // Unique ID for the log
        public string LogId { get; set; } = Guid.NewGuid().ToString();

        // Type of log
        public string LogType { get; set; }

        // Source of the log
        public string Source { get; set; }

        // Description of the event
        public string Message { get; set; }

        // Date and time the event occurred
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        // User related to the event
        public string UserIdentifier { get; set; }

        // Additional information about the event
        public string AdditionalInfo { get; set; }

        // Converts the log entry into text for Azure Files
        public override string ToString()
        {
            return $"[{LoggedAt:yyyy-MM-dd HH:mm:ss}] {LogType} | {Source} | {Message} | User: {UserIdentifier}";
        }
    }
}