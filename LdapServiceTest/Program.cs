
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// using System.Threading.Tasks;
// using Microsoft.Extensions.Hosting;



// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting/Internal/WebHost.cs
// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting.Abstractions/HostingAbstractionsHostExtensions.cs
// https://www.codewall.co.uk/running-net-core-generic-host-as-a-windows-service-linux-daemon-or-console-app/


// https://dejanstojanovic.net/aspnet/2018/august/creating-windows-service-and-linux-daemon-with-the-same-code-base-in-net/
namespace LdapServiceTest
{


    public interface ICommonService
    {
        void OnStart();
        void OnStop();
    }


    /*
    public abstract class CommonServiceBase
        : ICommonService
    {
        
        private IConfiguration configuration;
        ILogger<CommonServiceBase> logger;

        public IConfiguration Configuration => this.configuration;
        public ILogger<CommonServiceBase> Logger => this.logger;


        public CommonServiceBase(
              IConfiguration configuration
            , ILogger<CommonServiceBase> logger
            , MyConfig config
        )
        {
            this.configuration = configuration;
            this.logger = logger;
            System.Console.WriteLine(config.A);
            System.Console.WriteLine(config.B);
        }

        public abstract void OnStart();

        public abstract void OnStop();

       
        //void ICommonService.OnStart()
        //{
        //    throw new NotImplementedException();
        //}

        //void ICommonService.OnStop()
        //{
        //    throw new NotImplementedException();
        //}
        
    }
    */

    public class CommonSampleService 
        // : CommonServiceBase

        : ICommonService
{

        private IConfiguration configuration;
        ILogger<CommonSampleService> logger;

        
        public IConfiguration Configuration => this.configuration;
        public ILogger<CommonSampleService> Logger => this.logger;


        public CommonSampleService(
              IConfiguration configuration
            , ILogger<CommonSampleService> logger
            , MyConfig config
            , Microsoft.Extensions.Options.IOptions<SmtpConfig> smtp
            )  //: base(configuration, logger, config)
        {
            System.Console.WriteLine(smtp.Value.Server);
            this.configuration = configuration;
            this.logger = logger;
            System.Console.WriteLine(config.A);
            System.Console.WriteLine(config.B);

            logger.LogInformation("Class instatiated");
        }



        public bool m_Run;

        public async System.Threading.Tasks.Task RunDbSync()
        {
            while (this.m_Run)
            {
                StaticTestLogger.AppendLine("XXXService: Tick");
                System.Console.WriteLine("XXXService: Tick");
                await System.Threading.Tasks.Task.Delay(1000);
            } // Whend 

        } // End Task RunDbSync 



        //public override void OnStart()
        //{
        //    StaticTestLogger.AppendLine("XXXService: StartAsync");
        //    System.Console.WriteLine("XXXService: StartAsync");
        //    this.Logger.LogInformation("CommonSampleService OnStart");

        //    this.m_Run = true;
        //    System.Threading.Tasks.Task t = RunDbSync();
        //}


        //public override void OnStop()
        //{
        //    StaticTestLogger.AppendLine("XXXService: StopAsync");
        //    System.Console.WriteLine("XXXService: StopAsync");
        //    this.Logger.LogInformation("CommonSampleService OnStop");

        //    this.m_Run = false;
        //}


        void ICommonService.OnStart()
        {
            StaticTestLogger.AppendLine("XXXService: StartAsync");
            System.Console.WriteLine("XXXService: StartAsync");
            this.Logger.LogInformation("CommonSampleService OnStart");

            this.m_Run = true;
            System.Threading.Tasks.Task t = RunDbSync();
        }

        void ICommonService.OnStop()
        {
            StaticTestLogger.AppendLine("XXXService: StopAsync");
            System.Console.WriteLine("XXXService: StopAsync");
            this.Logger.LogInformation("CommonSampleService OnStop");

            this.m_Run = false;
        }



    }



    public partial class GenericService 
        : System.ServiceProcess.ServiceBase
    {
        ICommonService commonService;


        public GenericService(ICommonService commonService)
        {
            this.commonService = commonService;

            // InitializeComponent();
        }

        
        internal void StartService(string[] args)
        {
            this.commonService.OnStart();
        }
        

        protected override void OnStart(string[] args)
        {
            this.StartService(args);
        }


        protected override void OnStop()
        {
            this.commonService.OnStop();
        }
        

        // TODO: Implement
        protected override void OnPause()
        {

        }
        

        protected override void OnContinue()
        {

        }


        /*
        protected virtual void OnCustomCommand(int command);
        protected virtual bool OnPowerEvent(PowerBroadcastStatus powerStatus);
        protected virtual void OnSessionChange(SessionChangeDescription changeDescription);
        protected virtual void OnShutdown();
        protected virtual void OnStart(string[] args);
        protected virtual void OnStop();
        */


    } // End Class GenericService 

    class Program
    {
        // static void Main(string[] args) { Console.WriteLine("Running."); MainTask(args).Wait(); Console.WriteLine("Finished."); }


        // static async System.Threading.Tasks.Task MainTask(string[] args)
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            StaticTestLogger.ResetLogfile();
            
            
            IServiceCollection services = new ServiceCollection();

            // Create configuration builder  
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json")
            ;

            
            
            
            services.Configure<SmtpConfig>(
                delegate(SmtpConfig config) 
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


            // Inject configuration  
            services.AddSingleton<IConfiguration>(provider =>
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
            
            // Inject concrete implementaion of the service  
            services.AddSingleton(typeof(System.ServiceProcess.ServiceBase), typeof(GenericService));
            
            // My configuration
            services.AddSingleton(new MyConfig());
            
            
            // Build DI provider  
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            
            
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Console Debug mode  

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
                    
                    if(cc == System.ConsoleKey.C)
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
        } // End Task Main 
        

        static async System.Threading.Tasks.Task LinuxMain(string[] args)
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

            await host.RunAsync();
        } // End Task LinuxMain 


    } // End Class Program 







    public class LinuxServiceHost
        : IHostedService
    {

        
        IApplicationLifetime appLifetime;
        ILogger<LinuxServiceHost> logger;
        IHostingEnvironment environment;
        IConfiguration configuration;
        ICommonService commonService;


        public LinuxServiceHost(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<LinuxServiceHost> logger,
            IApplicationLifetime appLifetime,
            ICommonService commonService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.environment = environment;
            this.commonService = commonService;
        }

        
        private void OnStarted()
        {
            this.commonService.OnStart();
        }

        private void OnStopping()
        {
        }


        private void OnStopped()
        {
            this.commonService.OnStop();
        }
        

        Task IHostedService.StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            this.logger.LogInformation("StartAsync method called.");

            this.appLifetime.ApplicationStarted.Register(OnStarted);
            this.appLifetime.ApplicationStopping.Register(OnStopping);
            this.appLifetime.ApplicationStopped.Register(OnStopped);

            return System.Threading.Tasks.Task.CompletedTask;
        }


        Task IHostedService.StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }


    } // End Class LinuxServiceHost 


} // End Namespace 
