namespace SqlToGraphite.Plugin.CCTray
{
    using System;

    public class HttpException : Exception
    {
        public HttpException(string message) : base(message)
        {
        }
    }
}