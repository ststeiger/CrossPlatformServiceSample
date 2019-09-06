
using Microsoft.Extensions.Logging; // for LogInformation, LogError, LogWarning, etc.


namespace UniversalService
{
    
    
    public interface ICommonService
    {
        void OnStart();
        void OnStop();
        
        // TODO: Implement
        void OnPause();
        void OnContinue();
    } // End Interface ICommonService 
    
    
    public class LinuxServiceHost
        : Microsoft.Extensions.Hosting.IHostedService
    {
        Microsoft.Extensions.Hosting.IApplicationLifetime appLifetime;
        Microsoft.Extensions.Logging.ILogger<LinuxServiceHost> logger;
        Microsoft.Extensions.Hosting.IHostingEnvironment environment;
        Microsoft.Extensions.Configuration.IConfiguration configuration;
        ICommonService commonService;
        
        
        public LinuxServiceHost(
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            Microsoft.Extensions.Hosting.IHostingEnvironment environment,
            Microsoft.Extensions.Logging.ILogger<LinuxServiceHost> logger,
            Microsoft.Extensions.Hosting.IApplicationLifetime appLifetime,
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
        
        
        System.Threading.Tasks.Task 
            Microsoft.Extensions.Hosting.IHostedService
            .StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            this.logger.LogInformation("StartAsync method called.");

            this.appLifetime.ApplicationStarted.Register(OnStarted);
            this.appLifetime.ApplicationStopping.Register(OnStopping);
            this.appLifetime.ApplicationStopped.Register(OnStopped);

            return System.Threading.Tasks.Task.CompletedTask;
        }
        
        
        System.Threading.Tasks.Task 
            Microsoft.Extensions.Hosting.IHostedService
            .StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
        
        
    } // End Class LinuxServiceHost 
    
    
}