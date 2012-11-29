namespace SqlToGraphite.Plugin.CCTray
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;

    public class HttpGet : IHttpGet
    {
        private HttpWebRequest _request;
        private HttpWebResponse _response;

        private string _responseBody;
        private int _statusCode;
        private double _responseTime;
        private int _timeoutInSeconds;
        private string _username;
        private string _password;
        private bool _debug;

        public string ResponseBody { get { return this._responseBody; } }

        public int StatusCode { get { return this._statusCode; } }

        public double ResponseTime { get { return this._responseTime; } }

        public HttpGet(int timeoutInSeconds,string username,string password)
        {
            this._timeoutInSeconds = timeoutInSeconds;
            this._username = username;
            this._password = password;
            this._debug = false;
        }

        public HttpGet(int timeoutInSeconds, string username, string password,bool debug) : this(timeoutInSeconds, username, password)
        {
            this._debug = debug;
        }

        public void Request(string url)
        {
            var timer = new Stopwatch();
            
            this._request = (HttpWebRequest)WebRequest.Create(url);
            this._request.ReadWriteTimeout = (AsMiliSeconds(this._timeoutInSeconds));
            this._request.Timeout = (AsMiliSeconds(this._timeoutInSeconds));
            this.AddAuthorisationHeader();
            timer.Start();
            this._response = (HttpWebResponse)this._request.GetResponse();
            var buf = new byte[8192];
            Stream respStream = this._response.GetResponseStream();            
            this.GetData(timer, respStream, buf);
            timer.Stop();           
            this._statusCode = (int)this._response.StatusCode;
            this._responseTime = timer.ElapsedMilliseconds / 1000.0;
            if(this._debug)
            {
                Console.WriteLine();
                Console.WriteLine("Response Time:" + this._responseTime);
                Console.WriteLine("status code  :" + this._statusCode);
            }            
        }

        private void GetData(Stopwatch timer, Stream respStream, byte[] buf)
        {
            var respBody = new StringBuilder();
            int count;
            do
            {
                count = respStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    var data = Encoding.ASCII.GetString(buf, 0, count);
                    respBody.Append(data);
                    if (this._debug)
                    {
                        Console.Write(data);    
                    }                    
                }
                if (timer.ElapsedMilliseconds > AsMiliSeconds(this._timeoutInSeconds))
                {
                    throw new ApplicationException("Response time out exceeded");
                }
            }
            while (count > 0);
            this._responseBody = respBody.ToString();
        }

        private void AddAuthorisationHeader()
        {
            byte[] authBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}",this._username,this._password).ToCharArray());
            this._request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);
        }

        private static int AsMiliSeconds(int timeoutSeconds)
        {
            return timeoutSeconds * 1000;
        }      
    }
}