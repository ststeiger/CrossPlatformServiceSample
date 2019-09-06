
using Microsoft.Extensions.Logging; // for AddConsole, AddDebug, AddFilter, SetMinimumLevel
using Microsoft.Extensions.Configuration; // for SetBasePath, AddEnvironmentVariables, AddCommandLine, AddJsonFile 


// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting/Internal/WebHost.cs
// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting.Abstractions/HostingAbstractionsHostExtensions.cs
// https://www.codewall.co.uk/running-net-core-generic-host-as-a-windows-service-linux-daemon-or-console-app/


// https://dejanstojanovic.net/aspnet/2018/august/creating-windows-service-and-linux-daemon-with-the-same-code-base-in-net/
namespace LdapServiceTest
{


    class Program
    {


        // static void Main(string[] args) { Console.WriteLine("Running."); MainTask(args).Wait(); Console.WriteLine("Finished."); }

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            await RunService(args);
        } // End Task Main 


        static async System.Threading.Tasks.Task RunService(string[] args)
        {
            StaticTestLogger.ResetLogfile();

            var shb = new UniversalService.ServiceHostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    string dir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                    dir = System.IO.Directory.GetCurrentDirectory();

                    configHost.SetBasePath(dir);
                    configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                    configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                    configApp.AddJsonFile($"appsettings.json", true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddCommandLine(args);

                    // configApp.AddIniFile("");

                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.SetMinimumLevel(LogLevel.Information);
                    configLogging.AddFilter(x => x >= LogLevel.Trace);

                    configLogging.AddConsole();
                    configLogging.AddDebug();

                    // configLogging.AddSerilog(new LoggerConfiguration()
                    //           .ReadFrom.Configuration(hostContext.Configuration)
                    //           .CreateLogger());
                }).UseStartUp<ServiceStartup>()
                .Build();
            ;

            try
            {
                await shb.StartAsync(new System.Threading.CancellationToken());
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
            }

        } // End Task RunService 


    } // End Class Program 
    
    
} // End Namespace 
