
namespace LdapServiceTest
{

    public class HostingEnvironment : //IHostingEnvironment, 
        Microsoft.Extensions.Hosting.IHostingEnvironment
    {
        public string EnvironmentName { get; set; } // = Hosting.EnvironmentName.Production;

        public string ApplicationName { get; set; }

        public string WebRootPath { get; set; }

        public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; }

        public string ContentRootPath { get; set; }

        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
    }


}
