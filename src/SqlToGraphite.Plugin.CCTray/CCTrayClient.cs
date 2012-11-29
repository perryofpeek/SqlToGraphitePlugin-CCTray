using System;
using System.Collections.Generic;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.CCTray
{
    public class CCTrayClient : PluginBase
    {
        private string password;

        [Help("Name of the metric")]
        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public string Url { get; set; }

        public int TimeoutInSeconds { get; set; }

        public string Username { get; set; }

        [Encrypted]
        public string Password
        {
            get
            {
                return this.Encrypt(this.password);
            }

            set
            {
                this.password = this.Decrypt(value);
            }
        }

        [Help("Namespace path for the metric in graphite, use %h to substitute the hostname")]
        public string Path { get; set; }

        public CCTrayClient()
        {           
        }

        public CCTrayClient(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        public override IList<IResult> Get()
        {
            var rtn = new List<IResult>();

            try
            {               
                var tray = new CcTray(new EndpointImpl(new HttpGet(this.TimeoutInSeconds, this.Username, this.password), this.Url));
                tray.Load();   
                var now = DateTime.Now;                
                rtn.Add(this.TotalFailed(tray, now));
                rtn.Add(this.Total(tray, now));
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message, ex);
            }


            return rtn;
        }

        private Result TotalFailed(CcTray tray, DateTime now)
        {
            var totalFailed = new Result("TotalFailed", now, this.Path);
            totalFailed.SetValue(tray.FailedPipelines().Count);
            return totalFailed;
        }

        private Result Total(CcTray tray, DateTime now)
        {
            var totalFailed = new Result("Total", now, this.Path);
            totalFailed.SetValue(tray.AllPipelines().Count);
            return totalFailed;
        }
    }
}