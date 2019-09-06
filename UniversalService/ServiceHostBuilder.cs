
// for AddLogging, AddSingleton, Configure, BuildServiceProvider, AddHostedService
using Microsoft.Extensions.DependencyInjection;


namespace UniversalService
{


    public class ServiceHostBuilder
        : Microsoft.Extensions.Hosting.IHostBuilder
    {
        protected System.Collections.Generic.IDictionary<object, object> m_properties;
        System.Collections.Generic.IDictionary<object, object> Microsoft.Extensions.Hosting.IHostBuilder.Properties => this.m_properties;

        
        protected ApplicationBuilder m_applicationBuilder;
        protected Microsoft.Extensions.Configuration.IConfigurationBuilder m_configurationBuilder;
        protected Microsoft.Extensions.DependencyInjection.IServiceCollection m_hostServices;

        protected IStartup m_startup;

        protected Microsoft.Extensions.Hosting.HostBuilderContext m_context;
        protected Microsoft.Extensions.Hosting.IHostBuilder m_hostBuilder;
        protected Microsoft.Extensions.Hosting.IHost m_host;




        // https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting/Internal/ServiceCollectionExtensions.cs
        private static Microsoft.Extensions.DependencyInjection.IServiceCollection 
            Clone(Microsoft.Extensions.DependencyInjection.IServiceCollection serviceCollection)
        {
            Microsoft.Extensions.DependencyInjection.IServiceCollection clone = 
                new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            foreach (Microsoft.Extensions.DependencyInjection.ServiceDescriptor service in serviceCollection)
            {
                clone.Add(service);
            }

            return clone;
        }


        public ServiceHostBuilder()
        {
            this.m_properties = new System.Collections.Generic.Dictionary<object, object>();
            this.m_context = new Microsoft.Extensions.Hosting.HostBuilderContext(this.m_properties);
            this.m_context.HostingEnvironment = new HostingEnvironment();

            this.m_hostServices = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            this.m_configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();

            this.m_hostBuilder = new Microsoft.Extensions.Hosting.HostBuilder();
        }


        Microsoft.Extensions.Hosting.IHost Microsoft.Extensions.Hosting.IHostBuilder
            .Build()
        {
            ((IApplicationBuilder)this.m_applicationBuilder).ApplicationServices = this.m_host.Services;

            return new UniversalServiceHost(this.m_host);
        }


        Microsoft.Extensions.Hosting.IHostBuilder Microsoft.Extensions.Hosting.IHostBuilder
            .ConfigureContainer<TContainerBuilder>(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            this.m_hostBuilder.ConfigureContainer(configureDelegate);
            // configureDelegate(this.m_context, configureDelegate);

            return this;
        }

        Microsoft.Extensions.Hosting.IHostBuilder Microsoft.Extensions.Hosting.IHostBuilder
            .ConfigureHostConfiguration(
            System.Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configureDelegate)
        {
            this.m_hostBuilder.ConfigureHostConfiguration(configureDelegate);
            configureDelegate(this.m_configurationBuilder);
            return this;
        }

        Microsoft.Extensions.Hosting.IHostBuilder Microsoft.Extensions.Hosting.IHostBuilder
            .ConfigureAppConfiguration(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, 
                Microsoft.Extensions.Configuration.IConfigurationBuilder> configureDelegate)
        {
            this.m_hostBuilder.ConfigureAppConfiguration(configureDelegate);
            configureDelegate(this.m_context, this.m_configurationBuilder);
            return this;
        }


        Microsoft.Extensions.Hosting.IHostBuilder Microsoft.Extensions.Hosting.IHostBuilder
            .ConfigureServices(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, 
                Microsoft.Extensions.DependencyInjection.IServiceCollection> configureDelegate)
        {
            this.m_hostBuilder.ConfigureServices(configureDelegate);
            return this;
        }


        public ServiceHostBuilder ConfigureHostConfiguration(
            System.Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configureDelegate)
        {
            return (ServiceHostBuilder)((Microsoft.Extensions.Hosting.IHostBuilder)this).ConfigureHostConfiguration(configureDelegate);
        }


        public ServiceHostBuilder ConfigureAppConfiguration(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, 
                Microsoft.Extensions.Configuration.IConfigurationBuilder> configureDelegate)
        {
            return (ServiceHostBuilder)((Microsoft.Extensions.Hosting.IHostBuilder)this).ConfigureAppConfiguration(configureDelegate);
        }


        public ServiceHostBuilder ConfigureServices(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, 
            Microsoft.Extensions.DependencyInjection.IServiceCollection> configureDelegate)
        {
            return (ServiceHostBuilder)((Microsoft.Extensions.Hosting.IHostBuilder)this).ConfigureServices(configureDelegate);
        }


        Microsoft.Extensions.Hosting.IHostBuilder Microsoft.Extensions.Hosting.IHostBuilder
            .UseServiceProviderFactory<TContainerBuilder>(
            Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<TContainerBuilder> factory)
        {
            this.m_hostBuilder.UseServiceProviderFactory<TContainerBuilder>(factory);

            return this;
        }


        public ServiceHostBuilder ConfigureLogging(
            System.Action<Microsoft.Extensions.Hosting.HostBuilderContext, 
                Microsoft.Extensions.Logging.ILoggingBuilder> configureLogging)
        {
            this.m_hostBuilder = Microsoft.Extensions.Hosting.HostingHostBuilderExtensions
                .ConfigureLogging(this.m_hostBuilder, configureLogging);

            return this;
        }


        public ServiceHostBuilder ConfigureLogging(System.Action<Microsoft.Extensions.Logging.ILoggingBuilder> configureLogging)
        {
            this.m_hostBuilder = Microsoft.Extensions.Hosting.HostingHostBuilderExtensions
                .ConfigureLogging(this.m_hostBuilder, configureLogging);

            return this;
        }


        public Microsoft.Extensions.Hosting.IHostBuilder UseStartUp<T>()
        {
            System.Type startupType = typeof(T);

            this.m_hostServices.AddSingleton(typeof(IStartup), startupType);
            Microsoft.Extensions.Configuration.IConfiguration Configuration = this.m_configurationBuilder.Build();

            this.m_hostServices.Configure<Microsoft.Extensions.Configuration.IConfiguration>(Configuration);
            this.m_hostServices.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(Configuration);


            System.IServiceProvider sp = this.m_hostServices.BuildServiceProvider();

            // Microsoft.Extensions.DependencyInjection.IServiceCollection services2 = Clone(this.m_hostServices);

            this.m_hostBuilder.ConfigureServices(
                    (hostContext, services) =>
                    {
                        if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                            services.AddHostedService<LinuxServiceHost>();
                        else
                            // Inject concrete implementation of the service  
                            services.AddSingleton(typeof(System.ServiceProcess.ServiceBase), typeof(GenericService));
                    }
            );


            this.m_startup = sp.GetRequiredService<IStartup>();

            this.m_hostBuilder.ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.AddSingleton<IStartup>(this.m_startup);
                    }
            );


            this.m_hostBuilder.ConfigureServices((hostContext, services) =>
            {
                this.m_startup.ConfigureServices(services);
            });

            this.m_host = this.m_hostBuilder.Build();

            this.m_applicationBuilder = new ApplicationBuilder(this.m_host.Services);
            this.m_startup.Configure(this.m_applicationBuilder);

            return this;
        }


    } // End Class ServiceHostBuilder


}
