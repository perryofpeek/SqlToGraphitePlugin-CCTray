﻿namespace SqlToGraphite.Plugin.CCTray
{
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    public class Project : IProject
    {
        public Project()
        {
        }

        public Project(string name, string activity, string lastBuildStatus, string lastBuildLabel, string lastBuildTime, string webUrl)
        {
            this.name = name;
            this.activity = activity;
            this.lastBuildStatus = lastBuildStatus;
            this.lastBuildLabel = lastBuildLabel;
            this.lastBuildTime = lastBuildTime;
            this.webUrl = webUrl;
            this._pipelineName = name.Trim();
            if (name.IndexOf("::") > 0)
            {
                this._pipelineName = name.Substring(0, name.IndexOf("::")).Trim();
            }
        }

        [XmlAttribute]
        public string name { get;  set; }

        [XmlAttribute]
        public string activity { get;  set; }

        [XmlAttribute]
        public string lastBuildStatus { get;  set; }

        [XmlAttribute]
        public string lastBuildLabel { get;  set; }

        [XmlAttribute]
        public string lastBuildTime { get;  set; }

        [XmlAttribute]
        public string webUrl { get;  set; }

        public string PipelineName
        {
            get { return this._pipelineName; }
        }

        private string _pipelineName;

        public string GetMessage()
        {
            return string.Format("{0} has {2} build {1} {3}", this._pipelineName, this.lastBuildLabel, this.activity, this.webUrl);
        }

        public string GetUniqueKey()
        {
            return string.Format("{0}-{1}-{2}-{3}", this.PipelineName, this.lastBuildLabel, this.lastBuildTime, this.lastBuildStatus);
        }
    }
}