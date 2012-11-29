namespace SqlToGraphite.Plugin.CCTray
{
    public class EndpointImpl : IEndPoint
    {
        private readonly IHttpGet _httpGet;
        private readonly string _url;

        public EndpointImpl(IHttpGet httpGet, string url)
        {
            this._httpGet = httpGet;
            this._url = url;
        }

        public string GetXml()
        {
            this._httpGet.Request(this._url);
            if (this._httpGet.StatusCode != 200)
            {
                throw new HttpException(string.Format("Http status code is {0}", this._httpGet.StatusCode));
            }
            return this._httpGet.ResponseBody;
        }
    }
}