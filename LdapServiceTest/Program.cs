
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// using System.Threading.Tasks;
// using Microsoft.Extensions.Hosting;



// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting/Internal/WebHost.cs
// https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting.Abstractions/HostingAbstractionsHostExtensions.cs
// https://www.codewall.co.uk/running-net-core-generic-host-as-a-windows-service-linux-daemon-or-console-app/


// https://dejanstojanovic.net/aspnet/2018/august/creating-windows-service-and-linux-daemon-with-the-same-code-base-in-net/
namespace LdapServiceTest
{


    public class MyConfig
    {
        public string A = "AAAAA";
        public string B = "B";
    }


    static class StaticTestLogger
    {
        

        private static string FileName
        {
            get
            {
                if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                    return @"d:\Dummy.log.txt";

                return "/root/Documents/Dummy.log.txt";
            }
        }


        public static void AppendLine(string text)
        {
            System.IO.File.AppendAllText(FileName, text + System.Environment.NewLine);
        }

        public static void ResetLogfile()
        {
            if (System.IO.File.Exists(FileName))
                System.IO.File.Delete(FileName);

        }

    }


    public interface ICommonService
    {
        void OnStart();
        void OnStop();
    }


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

        /*
        void ICommonService.OnStart()
        {
            throw new NotImplementedException();
        }

        void ICommonService.OnStop()
        {
            throw new NotImplementedException();
        }
        */
    }


    public class CommonSampleService 
        : CommonServiceBase
    {

        public CommonSampleService(
              IConfiguration configuration
            , ILogger<CommonSampleService> logger
            , MyConfig config
            ) : base(configuration, logger, config)
        {
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
            }

        }



        public override void OnStart()
        {
            StaticTestLogger.AppendLine("XXXService: StartAsync");
            System.Console.WriteLine("XXXService: StartAsync");
            this.Logger.LogInformation("CommonSampleService OnStart");

            this.m_Run = true;
            System.Threading.Tasks.Task t = RunDbSync();
        }

        public override void OnStop()
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


        /*
         
        protected virtual void OnContinue();
        protected virtual void OnCustomCommand(int command);
        protected virtual void OnPause();
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


        //static async System.Threading.Tasks.Task MainTask(string[] args)
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            StaticTestLogger.ResetLogfile();
            
            #region Dependecy injection setup  
            IServiceCollection services = new ServiceCollection();

            //Create configuration builder  
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json")
            ;


            //Inject configuration  
            services.AddSingleton<IConfiguration>(provider =>
            {
                return configurationBuilder.Build();
            });

            //Inject Serilog  
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

            //Inject common service  
            services.AddSingleton(typeof(ICommonService), typeof(CommonSampleService));

            //Inject concrete implementaion of the service  
            services.AddSingleton(typeof(System.ServiceProcess.ServiceBase), typeof(GenericService));


            services.AddSingleton(new MyConfig());
            


            //Build DI provider  
            ServiceProvider serviceProvider = services.BuildServiceProvider();


            #endregion

            if (System.Diagnostics.Debugger.IsAttached)
            {
                //Console Debug mode  

                GenericService svc = serviceProvider.GetService<System.ServiceProcess.ServiceBase>() as GenericService;
                svc.StartService(args);

                System.Console.ReadLine();
            }
            else
            {
                //Start service  

                System.ServiceProcess.ServiceBase[] ServicesToRun;
                ServicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    serviceProvider.GetService<System.ServiceProcess.ServiceBase>()
                };

                System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            }

            await System.Threading.Tasks.Task.CompletedTask;
        }


    } // End Class Program 


} // End Namespace 
