using Serilog;
using Serilog.Formatting.Compact;
namespace Infra.Logging;
public static class SerilogConfiguration
{
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(new CompactJsonFormatter())
            .CreateLogger();
    }
}
