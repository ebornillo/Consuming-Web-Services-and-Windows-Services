using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatcherService.Services
{
    public class FolderWatcherService : BackgroundService
    {
        private readonly ILogger<FolderWatcherService> _logger;
        private readonly string _sourcePath;
        private readonly string _destinationPath;
        private FileSystemWatcher? _watcher;

        public FolderWatcherService(ILogger<FolderWatcherService> logger, IConfiguration configuration)
        {
            _logger = logger;

            _sourcePath = configuration["FolderSettings:SourceFolder"]
                ?? throw new InvalidOperationException("Missing SourceFolder config");
            _destinationPath = configuration["FolderSettings:DestinationFolder"]
                ?? throw new InvalidOperationException("Missing DestinationFolder config");

            Directory.CreateDirectory(_sourcePath);
            Directory.CreateDirectory(_destinationPath);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FileWatcherService started.");

            // Process any existing files at startup
            ProcessExistingFiles();

            // Initialize FileSystemWatcher for new files
            _watcher = new FileSystemWatcher(_sourcePath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
            };
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;

            stoppingToken.Register(() =>
            {
                _watcher?.Dispose();
                _logger.LogInformation("FileWatcherService stopping.");
            });

            return Task.CompletedTask;
        }

        private void ProcessExistingFiles()
        {
            try
            {
                var files = Directory.GetFiles(_sourcePath);
                foreach (var file in files)
                {
                    MoveFileWithRetry(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing existing files in {Folder}", _sourcePath);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            MoveFileWithRetry(e.FullPath);
        }

        private void MoveFileWithRetry(string sourceFile)
        {
            int retries = 5;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    string destFile = Path.Combine(_destinationPath, Path.GetFileName(sourceFile));
                    File.Move(sourceFile, destFile, true);
                    _logger.LogInformation("Moved file {File} to {Dest}", sourceFile, destFile);
                    break; // success, exit loop
                }
                catch (IOException)
                {
                    // File might be locked, wait and retry
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to move file: {File}", sourceFile);
                    break;
                }
            }
        }
    }
}
