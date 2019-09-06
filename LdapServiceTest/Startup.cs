
using Microsoft.Extensions.DependencyInjection;


namespace LdapServiceTest
{

    // namespace Microsoft.AspNet.Builder
    public interface IApplicationBuilder
    {
        System.IServiceProvider ApplicationServices { get; set; }
        object Server { get; }
        System.Collections.Generic.IDictionary<string, object> Properties { get; }
        
        // Microsoft.AspNet.Http.Abstractions RequestDelegate
        // IApplicationBuilder Use(System.Func<RequestDelegate, RequestDelegate> middleware);
        
        IApplicationBuilder New();

        // RequestDelegate Build();
    } // End interface IApplicationBuilder



    // namespace Microsoft.AspNetCore.Hosting
    public interface IStartup
    {
        System.IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services);

        void Configure(IApplicationBuilder app);
    } // End Interface IStartup
    
    
    public class ServiceStartup
        : IStartup
    {
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }
        private IApplicationBuilder m_application;
        
        
        public ServiceStartup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.Configuration = configuration;
        } // End Constructor 
        
        
        System.IServiceProvider IStartup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddLogging();

            // Inject common service  
            services.AddSingleton(typeof(ICommonService), typeof(CommonSampleService));

            

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


        void IStartup.Configure(IApplicationBuilder app)
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
