using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.ConsoleApp.Logging;

class CollectorConsoleLoggingProvider : ILoggerProvider
{
    public void Dispose()
    {
        // do nothing
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CollectorConsoleLogger(categoryName);
    }

    public class CollectorConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public CollectorConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            System.Console.WriteLine($"{formatter(state, exception)}");

            if (exception is not null)
            {
                System.Console.WriteLine(exception.StackTrace);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel >= LogLevel.Information);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default!;  // todo: IDisposable? in .net 7 - ! not needed
        }
    }
}
