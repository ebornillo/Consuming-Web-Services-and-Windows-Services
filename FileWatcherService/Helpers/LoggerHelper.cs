using Serilog;
using ILogger = Serilog.ILogger;

namespace FileWatcherService.Helpers
{
    public class LoggerHelper
    {
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
