using AccountService.API2.Configures;
using Serilog;

namespace AccountService.API2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .CreateLogger();

            Log.Information("Starting up");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((ctx, lc) => lc
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(ctx.Configuration));

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                var app = builder
                    .ConfigureServices()
                    .ConfigurePipeline();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex) when (
                                        ex.GetType().Name is not "StopTheHostException"
                                        && ex.GetType().Name is not "HostAbortedException"
                                    )
            {
                Log.Fatal(ex, "Unhandled exception");
            }
            finally
            {
                Log.Information("Shut down complete");
                Log.CloseAndFlush();
            }
        }
    }
}
