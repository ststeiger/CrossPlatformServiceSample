using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if false


namespace LdapServiceTest
{
    static class EnvironmentName
    {
        public static string FileName = @"d:\Dummy.log.txt";


        public static string Production = "Prod";
        public static string Development = "Dev";


        public static void AppendLine(string text)
        {
            System.IO.File.AppendAllText(FileName, text + System.Environment.NewLine);
        }


    }

    public class ServiceBaseLifetime
      : System.ServiceProcess.ServiceBase, Microsoft.Extensions.Hosting.IHostLifetime
    {
        Task IHostLifetime.StopAsync(CancellationToken cancellationToken)
        {
            EnvironmentName.AppendLine("ServiceBaseLifetime: StopAsync");

            System.Console.WriteLine("ServiceBaseLifetime: StopAsync");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        Task IHostLifetime.WaitForStartAsync(CancellationToken cancellationToken)
        {

            EnvironmentName.AppendLine("ServiceBaseLifetime: WaitForStartAsync");

            System.Console.WriteLine("ServiceBaseLifetime: WaitForStartAsync");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }


    public static class ServiceBaseLifetimeHostExtensions
    {
        public static Microsoft.Extensions.Hosting.IHostBuilder UseServiceBaseLifetime(
    this Microsoft.Extensions.Hosting.IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(
                (hostContext, services) => services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, ServiceBaseLifetime>()
            );
        }


        public static System.Threading.Tasks.Task RunAsServiceAsync(
              this Microsoft.Extensions.Hosting.IHostBuilder hostBuilder
            , System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }


    public class XXXService
        : IHostedService
    {

        public bool m_Run;

        public async Task RunDbSync()
        {
            while (this.m_Run)
            {
                EnvironmentName.AppendLine("XXXService: Tick");
                System.Console.WriteLine("XXXService: Tick");
                await System.Threading.Tasks.Task.Delay(1000);
            }

        }


        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            EnvironmentName.AppendLine("XXXService: StartAsync");
            System.Console.WriteLine("XXXService: StartAsync");
            this.m_Run = true;
            Task t = RunDbSync();

            return System.Threading.Tasks.Task.CompletedTask;
        }


        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            EnvironmentName.AppendLine("XXXService: StopAsync");
            System.Console.WriteLine("XXXService: StopAsync");

            this.m_Run = false;

            return System.Threading.Tasks.Task.Delay(2000);
        }
    }



    class NotWorkingProgram
    {
        // static void Main(string[] args) { Console.WriteLine("Hello World!"); }


        static async Task Main(string[] args)
        {
            if (System.IO.File.Exists(EnvironmentName.FileName))
                System.IO.File.Delete(EnvironmentName.FileName);
            
            // Run with console or service
            var asService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<XXXService>();
            });
            builder.UseEnvironment(asService ? EnvironmentName.Production :
            EnvironmentName.Development);

            if (asService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        } // End Task Main 


    } // End Class Program 


}
#endif