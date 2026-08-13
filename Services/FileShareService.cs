using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Options;
using RetailProject.Options;

namespace RetailProject.Services
{
    // Handles application logs stored in Azure File Share
    public class FileShareService
    {
        private readonly ShareClient _shareClient;
        private readonly ShareDirectoryClient _directoryClient;
        private readonly ILogger<FileShareService> _logger;

        public FileShareService(
            IOptions<AzureStorageOptions> options,
            ILogger<FileShareService> logger)
        {
            _logger = logger;

            var connectionString = options.Value.ConnectionString;
            var shareName = options.Value.LogFileShare;

            _shareClient = new ShareClient(connectionString, shareName);
            _directoryClient = _shareClient.GetRootDirectoryClient();
        }

        // Ensures the file share exists
        public async Task InitializeAsync()
        {
            await _shareClient.CreateIfNotExistsAsync();
        }

        // Writes a log entry to the Azure File Share as a .txt file
        public async Task WriteLogAsync(string logMessage)
        {
            try
            {
                string fileName = $"logs_{DateTime.UtcNow:yyyy-MM-dd}.txt";
                var fileClient = _directoryClient.GetFileClient(fileName);

                string formattedLog = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {logMessage}{Environment.NewLine}";

                // Check if the file exists
                bool exists = await fileClient.ExistsAsync();

                string contentToWrite = formattedLog;

                if (exists)
                {
                    // If it exists, download current text, append new text
                    var download = await fileClient.DownloadAsync();
                    using var reader = new StreamReader(download.Value.Content);
                    string existingContent = await reader.ReadToEndAsync();
                    contentToWrite = existingContent + formattedLog;
                }

                // Convert the final string to a byte stream
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(contentToWrite);
                using var stream = new MemoryStream(byteArray);

                // FIX: Create (or recreate) the file with the full content
                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadAsync(stream);

                _logger.LogInformation($"Log entry written to Azure File Share: {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write log to Azure File Share.");
            }
        }

        public async Task<string> ReadLogFileAsync()
        {
            try
            {
                string fileName = $"logs_{DateTime.UtcNow:yyyy-MM-dd}.txt";
                var fileClient = _directoryClient.GetFileClient(fileName);

                if (await fileClient.ExistsAsync())
                {
                    var download = await fileClient.DownloadAsync();
                    using var reader = new StreamReader(download.Value.Content);
                    return await reader.ReadToEndAsync();
                }
                return "No logs found for today.";
            }
            catch
            {
                return "Could not read log file from Azure.";
            }
        }
    }
}