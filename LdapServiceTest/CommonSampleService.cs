
using Microsoft.Extensions.Logging;


namespace LdapServiceTest
{
    
    
    public class CommonSampleService
        : ICommonService
    {
        
        
        protected Microsoft.Extensions.Configuration.IConfiguration m_configuration;
        protected Microsoft.Extensions.Logging.ILogger<CommonSampleService> m_logger;
        protected bool m_run;
        
        
        // public ILogger<CommonSampleService> Logger => this.m_logger;
        public Microsoft.Extensions.Logging.ILogger<CommonSampleService> Logger { get { return this.m_logger; } }
        
        public Microsoft.Extensions.Configuration.IConfiguration Configuration => this.m_configuration;
        
        
        public CommonSampleService(
            Microsoft.Extensions.Configuration.IConfiguration configuration
            , Microsoft.Extensions.Logging.ILogger<CommonSampleService> logger
            , MyConfig config
            , Microsoft.Extensions.Options.IOptions<SmtpConfig> smtp
            )
        {
            System.Console.WriteLine(smtp.Value.Server);
            this.m_configuration = configuration;
            this.m_logger = logger;

            System.Console.WriteLine(config.A);
            System.Console.WriteLine(config.B);

            logger.LogInformation("Class instatiated");
        }
        
        
        public async System.Threading.Tasks.Task RunDbSync()
        {
            while (this.m_run)
            {
                StaticTestLogger.AppendLine("XXXService: Tick");
                System.Console.WriteLine("XXXService: Tick");
                await System.Threading.Tasks.Task.Delay(1000);
            } // Whend 

        } // End Task RunDbSync 
        
        
        void ICommonService.OnStart()
        {
            StaticTestLogger.AppendLine("XXXService: StartAsync");
            System.Console.WriteLine("XXXService: StartAsync");
            this.Logger.LogInformation("CommonSampleService OnStart");
            this.m_run = true;
            System.Threading.Tasks.Task t = RunDbSync();
        }
        
        
        void ICommonService.OnStop()
        {
            StaticTestLogger.AppendLine("XXXService: StopAsync");
            System.Console.WriteLine("XXXService: StopAsync");
            this.Logger.LogInformation("CommonSampleService OnStop");

            this.m_run = false;
        }
        
        
        void ICommonService.OnPause()
        {
            ((ICommonService)this).OnStop();
        }
        
        
        void ICommonService.OnContinue()
        {
            ((ICommonService)this).OnStart();
        }
        
        
    } // End Class CommonSampleService 
    
    
}
