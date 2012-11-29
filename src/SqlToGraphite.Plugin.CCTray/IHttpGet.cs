namespace SqlToGraphite.Plugin.CCTray
{
    public interface IHttpGet
    {
        string ResponseBody { get; }
        
        int StatusCode { get; }
        
        double ResponseTime { get; }
        
        void Request(string url);
    }
}