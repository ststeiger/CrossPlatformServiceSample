
using Microsoft.Extensions.Hosting; // for RunAsync 
using Microsoft.Extensions.DependencyInjection; // for GetService


namespace LdapServiceTest
{


    public class UniversalServiceHost
        : Microsoft.Extensions.Hosting.IHost
    {


        protected Microsoft.Extensions.Hosting.IHost m_host;

        System.IServiceProvider Microsoft.Extensions.Hosting.IHost.Services => this.m_host.Services;


        public UniversalServiceHost(Microsoft.Extensions.Hosting.IHost host)
        {
            this.m_host = host;
        }

        public UniversalServiceHost()
            : this(null)
        { }


        void System.IDisposable.Dispose()
        {
            throw new System.NotImplementedException();
        }

        async System.Threading.Tasks.Task Microsoft.Extensions.Hosting.IHost
            .StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            string[] args = null;

            GenericService svc = this.m_host.Services.GetService<System.ServiceProcess.ServiceBase>() as GenericService;
            svc.StartService(args);

            // System.Console.ReadLine();
            System.ConsoleKey cc = default(System.ConsoleKey);
            do
            {
                // THIS IS MADNESS!!!   Madness huh?   THIS IS SPARTA!!!!!!! 
                while (!System.Console.KeyAvailable)
                {
                    // System.Threading.Thread.Sleep(100);
                    await System.Threading.Tasks.Task.Delay(100);
                }

                cc = System.Console.ReadKey().Key;

                if (cc == System.ConsoleKey.C)
                    System.Console.Clear();

            } while (cc != System.ConsoleKey.Enter);

            svc.Stop();
        }



        protected async System.Threading.Tasks.Task RunAsPlatformIndependentService(string[] args)
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                await RunAsDaemon(args);
            else
                await RunAsWindowsService(args);
        } // End Task RunAsPlatformIndependentService 


        // static async System.Threading.Tasks.Task MainTask(string[] args)
        protected async System.Threading.Tasks.Task RunAsWindowsService(string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Console Debug mode  
                GenericService svc = this.m_host.Services.GetService<System.ServiceProcess.ServiceBase>() as GenericService;
                svc.StartService(args);

                // System.Console.ReadLine();

                System.ConsoleKey cc = default(System.ConsoleKey);
                do
                {
                    // THIS IS MADNESS!!!   Madness huh?   THIS IS SPARTA!!!!!!! 
                    while (!System.Console.KeyAvailable)
                    {
                        // System.Threading.Thread.Sleep(100);
                        await System.Threading.Tasks.Task.Delay(100);
                    }

                    cc = System.Console.ReadKey().Key;

                    if (cc == System.ConsoleKey.C)
                        System.Console.Clear();

                } while (cc != System.ConsoleKey.Enter);

                svc.Stop();
            }
            else
            {
                // Start service 
                System.ServiceProcess.ServiceBase[] servicesToRun;
                servicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    this.m_host.Services.GetService<System.ServiceProcess.ServiceBase>()
                };

                System.ServiceProcess.ServiceBase.Run(servicesToRun);
            } // End else of if (System.Diagnostics.Debugger.IsAttached) 

            // await System.Threading.Tasks.Task.CompletedTask;
        } // End Task RunAsWindowsService 


        protected async System.Threading.Tasks.Task RunAsDaemon(string[] args)
        {
            // IServiceCollection isc = host.Services.GetRequiredService<IServiceCollection>();
            // IConfiguration confy = host.Services.GetRequiredService<IConfiguration>();

            await this.m_host.RunAsync();
        } // End Task RunAsDaemon 



        System.Threading.Tasks.Task Microsoft.Extensions.Hosting.IHost
            .StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


    } // End Class UniversalServiceHost 


}
