
namespace UniversalService
{


    // namespace Microsoft.AspNetCore.Hosting
    public interface IStartup
    {
        System.IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services);

        void Configure(IApplicationBuilder app);
    } // End Interface IStartup


}
