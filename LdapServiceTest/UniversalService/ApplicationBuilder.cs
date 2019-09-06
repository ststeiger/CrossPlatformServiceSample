
namespace LdapServiceTest
{


    public class ApplicationBuilder
        : IApplicationBuilder
    {
        protected System.Collections.Generic.IDictionary<string, object> m_applicationProperties;
        protected System.IServiceProvider m_serviceProvider;



        System.Collections.Generic.IDictionary<string, object> 
            IApplicationBuilder.Properties => this.m_applicationProperties;

        System.IServiceProvider IApplicationBuilder.ApplicationServices
        {
            get => this.m_serviceProvider; set => this.m_serviceProvider = value;
        }

        object IApplicationBuilder.Server => null;


        public ApplicationBuilder(System.IServiceProvider serviceProvider)
        {
            this.m_applicationProperties = new System.Collections.Generic.Dictionary<string, object>();
            this.m_serviceProvider = serviceProvider;
        }


        public ApplicationBuilder()
            : this(null)
        { }


        IApplicationBuilder IApplicationBuilder.New()
        {
            return new ApplicationBuilder();
        }


    }


}
