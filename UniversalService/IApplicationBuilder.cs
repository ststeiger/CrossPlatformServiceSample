
namespace UniversalService
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


}
