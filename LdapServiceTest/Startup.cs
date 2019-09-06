
using Microsoft.Extensions.DependencyInjection;


namespace LdapServiceTest
{


    public class ServiceStartup
        : UniversalService.IStartup
    {
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }
        private UniversalService.IApplicationBuilder m_application;
        
        
        public ServiceStartup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.Configuration = configuration;
        } // End Constructor 
        
        
        System.IServiceProvider UniversalService.IStartup
            .ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddLogging();

            // Inject common service  
            services.AddSingleton(typeof(UniversalService.ICommonService), typeof(CommonSampleService));

            

            // My configuration
            services.AddSingleton(new MyConfig());

            services.Configure<SmtpConfig>(
                delegate (SmtpConfig config)
                {
                    config.Server = "hello world";
                    return;
                }
            );

            return services.BuildServiceProvider();
        }


        void UniversalService.IStartup.Configure(UniversalService.IApplicationBuilder app)
        {
            this.m_application = app;
            
            // Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = app.ApplicationServices.
            //     GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
            
            // Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.ApplicationServices.
            //     GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
            
            // throw new NotImplementedException();
        }
        
        
    } // End Class ServiceStartup 
    
    
} // End Namespace LdapServiceTest 
