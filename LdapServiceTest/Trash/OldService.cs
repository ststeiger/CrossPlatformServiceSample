
#if false

using Microsoft.Extensions.Logging; // for AddConsole, AddDebug, AddFilter, SetMinimumLevel
using Microsoft.Extensions.Hosting; // for ConfigureLogging, RunAsync 
using Microsoft.Extensions.Configuration; // for SetBasePath, AddEnvironmentVariables, AddCommandLine, AddJsonFile 

// for AddLogging, AddSingleton, Configure, BuildServiceProvider, AddHostedService
using Microsoft.Extensions.DependencyInjection;


namespace LdapServiceTest.Trash
{


    class OldService
    {


        static async System.Threading.Tasks.Task RunAsPlatformIndependentService(string[] args)
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                await RunAsDaemon(args);
            else
                await RunAsWindowsService(args);
        } // End Task RunAsPlatformIndependentService 


        // static async System.Threading.Tasks.Task MainTask(string[] args)
        static async System.Threading.Tasks.Task RunAsWindowsService(string[] args)
        {
            Microsoft.Extensions.DependencyInjection.IServiceCollection services =
                new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            // Create configuration builder  
            Microsoft.Extensions.Configuration.IConfigurationBuilder configurationBuilder =
                    new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            // .AddIniFile(@"D:\inetpub\LdapService\LdapService.ini")
            //.AddJsonFile("appsettings.json")
            ;

            // Inject configuration  
            services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(provider =>
            {
                return configurationBuilder.Build();
            });


            // Inject Serilog  
            services.AddLogging(options =>
            {
                options.SetMinimumLevel(LogLevel.Information);
                options.AddFilter(x => x >= LogLevel.Trace);

                options.AddConsole();
                options.AddDebug();

                /*
                options.AddSerilog(
                    new LoggerConfiguration()
                               .ReadFrom.Configuration(configurationBuilder.Build())
                               .CreateLogger()
                    );
                */
            });


            // Inject common service  
            services.AddSingleton(typeof(ICommonService), typeof(CommonSampleService));

            // Inject concrete implementation of the service  
            services.AddSingleton(typeof(System.ServiceProcess.ServiceBase), typeof(GenericService));

            // My configuration
            services.AddSingleton(new MyConfig());

            services.Configure<SmtpConfig>(
                delegate (SmtpConfig config)
                {
                    config.Server = "hello world";
                    return;
                }
            );


            /*
            // IConfiguration Configuration = null;
            
            // services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));
            // IConfiguration iconf = Configuration.GetSection("Smtp");
            Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<SmtpConfig>(
                services, iconf
            );
            */


            // Build DI provider  
            Microsoft.Extensions.DependencyInjection.ServiceProvider serviceProvider = services.BuildServiceProvider();


            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Console Debug mode  

                /*
                try
                {
                    IConfiguration confy = serviceProvider.GetService<IConfiguration>();
                    System.Console.WriteLine(confy);
                    // var sec = confy.GetSection("ErrorMail2");
                    // https://stackoverflow.com/questions/39169701/how-to-extract-a-list-from-appsettings-json-in-net-core
                    // foreach (var section in confy.GetChildren()) section.GetChildren();


                    // Fixes to IniStreamConfigurationProvider.cs:if (separator < 0)
                    var jobs = confy.GetSection("Jobs").Get<System.Collections.Generic.Dictionary<string, string>>();
                    var sec = confy.GetSection("ErrorMail2").Get<System.Collections.Generic.Dictionary<string, string>>();

                    // sec.Keys, sec.Values

                    System.Console.WriteLine(sec);
                    System.Console.WriteLine(jobs);


                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                */



                GenericService svc = serviceProvider.GetService<System.ServiceProcess.ServiceBase>() as GenericService;
                svc.StartService(args);

                // System.Console.ReadLine();

                System.ConsoleKey cc = default(System.ConsoleKey);
                do
                {
                    // THIS IS MADNESS!!!   Madness huh?   THIS IS SPARTA!!!!!!! 
                    while (!System.Console.KeyAvailable)
                    {
                        // System.Threading.Thread.Sleep(100);
                        await System.Threading.Tasks.Task.Delay(100);
                    }

                    cc = System.Console.ReadKey().Key;

                    if (cc == System.ConsoleKey.C)
                        System.Console.Clear();

                } while (cc != System.ConsoleKey.Enter);

                svc.Stop();
            }
            else
            {
                // Start service 
                System.ServiceProcess.ServiceBase[] servicesToRun;
                servicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    serviceProvider.GetService<System.ServiceProcess.ServiceBase>()
                };

                System.ServiceProcess.ServiceBase.Run(servicesToRun);
            } // End else of if (System.Diagnostics.Debugger.IsAttached) 

            // await System.Threading.Tasks.Task.CompletedTask;
        } // End Task RunAsWindowsService 


        static async System.Threading.Tasks.Task RunAsDaemon(string[] args)
        {

            Microsoft.Extensions.Hosting.IHost host =
                new Microsoft.Extensions.Hosting.HostBuilder()
                 .ConfigureHostConfiguration(configHost =>
                 {
                     configHost.SetBasePath(System.IO.Directory.GetCurrentDirectory());
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
                 })
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddLogging();
                     services.AddHostedService<LinuxServiceHost>();
                     services.AddSingleton(typeof(ICommonService), typeof(CommonSampleService));

                     // My configuration
                     services.AddSingleton(new MyConfig());
                 })
                 .ConfigureLogging((hostContext, configLogging) =>
                 {
                     // configLogging.AddSerilog(new LoggerConfiguration()
                     //           .ReadFrom.Configuration(hostContext.Configuration)
                     //           .CreateLogger());
                     configLogging.AddConsole();
                     configLogging.AddDebug();
                 })
                 .Build();


            // IServiceCollection isc = host.Services.GetRequiredService<IServiceCollection>();
            // IConfiguration confy = host.Services.GetRequiredService<IConfiguration>();


            // host.Services;

            await host.RunAsync();
        } // End Task RunAsDaemon 


    }
}

#endif
