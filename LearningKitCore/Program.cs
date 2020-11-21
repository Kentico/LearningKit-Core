using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace LearningKitCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // The UseServiceProviderFactory call attaches the
                // Autofac provider to the generic hosting mechanism.
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>();
                });
    }
}
