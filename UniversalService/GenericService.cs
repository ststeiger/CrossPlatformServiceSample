
namespace UniversalService
{
    
    
    public partial class GenericService 
        : System.ServiceProcess.ServiceBase
    {
        ICommonService commonService;
        
        
        public GenericService(ICommonService commonService)
        {
            this.commonService = commonService;
        }
        
        
        internal void StartService(string[] args)
        {
            this.commonService.OnStart();
        }
        
        
        protected override void OnStart(string[] args)
        {
            this.StartService(args);
        }
        
        
        protected override void OnStop()
        {
            this.commonService.OnStop();
        }
        
        
        protected override void OnPause()
        {
            this.commonService.OnPause();
        }
        
        
        protected override void OnContinue()
        {
            this.commonService.OnContinue();
        }
        
        // protected virtual void OnCustomCommand(int command);
        // protected virtual bool OnPowerEvent(PowerBroadcastStatus powerStatus);
        // protected virtual void OnSessionChange(SessionChangeDescription changeDescription);
        // protected virtual void OnShutdown();
        
    } // End Class GenericService 
    
    
}
