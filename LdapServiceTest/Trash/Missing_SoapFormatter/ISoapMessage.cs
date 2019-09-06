
namespace System.Runtime.Serialization.Formatters
{
    using System;


    [System.Runtime.InteropServices.ComVisible(true)]
    public interface ISoapMessage
    {
        String[] ParamNames {get; set;}
        Object[] ParamValues {get; set;}
        Type[] ParamTypes {get; set;}        
        String MethodName {get; set;}
        String XmlNameSpace {get; set;}
        //Header[] Headers {get; set;}
        System.Collections.IEnumerable Headers { get; set; }
    }
}
