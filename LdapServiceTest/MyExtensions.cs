
namespace LdapServiceTest
{
    class MyExtensions
    { }
    
    
    public class SmtpConfig
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
    }
    
    public class MyConfig
    {
        public string A = "AAAAA";
        public string B = "B";
    }
    
    
    static class StaticTestLogger
    {
        
        
        private static string FileName
        {
            get
            {
                if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                    return @"d:\Dummy.log.txt";

                return "/root/Documents/Dummy.log.txt";
            }
        }
        
        
        public static void AppendLine(string text)
        {
            System.IO.File.AppendAllText(FileName, text + System.Environment.NewLine);
        }
        
        
        public static void ResetLogfile()
        {
            if (System.IO.File.Exists(FileName))
                System.IO.File.Delete(FileName);
        }
        
        
    }
    
    
}
