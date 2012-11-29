﻿namespace SqlToGraphite.Plugin.CCTray
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class CcTray : ICcTray
    {
        private readonly IEndPoint _endPoint;
        private XDocument _document;

        public List<Project> FailedPipelines()
        {
            return this.GetProjectsWithLastBuildStatus("Failure");
        }

        public List<Project> AllPipelines()
        {
            return this.GetProjects();
        }

        public List<string> AllPipelineNames()
        {
            var rtn = new List<string>();
            foreach (var p in this.GetProjects())
            {
                if(!rtn.Contains(p.PipelineName))
                {
                    rtn.Add(p.PipelineName);
                }
            }
            rtn.Sort();
            return rtn;
        }

        public CcTray(IEndPoint endPoint)
        {
            this._endPoint = endPoint;
        }

        public void Load()
        {
            this._document = this.GetRemoteData();
        }

        private List<Project> GetProjects()
        {
            var x = (from c in this._document.Descendants("Projects").Descendants("Project")
                     let attribute = c.Attribute("lastBuildStatus")
                     where attribute != null
                     let xAttribute = c.Attribute("name")
                     where xAttribute != null
                     let xAttribute1 = c.Attribute("activity")
                     where xAttribute1 != null
                     let attribute1 = c.Attribute("lastBuildLabel")
                     where attribute1 != null
                     let xAttribute2 = c.Attribute("lastBuildTime")
                     where xAttribute2 != null
                     let attribute2 = c.Attribute("webUrl")
                     where attribute2 != null
                     select
                         new Project(xAttribute.Value,
                                     attribute.Value,
                                     xAttribute1.Value,
                                     attribute1.Value,
                                     xAttribute2.Value,
                                     attribute2.Value)
                    );
            return x.ToList();
        }


        private List<Project> GetProjectsWithLastBuildStatus(string status)
        {
            var x = (from c in this._document.Descendants("Projects").Descendants("Project")
                     let attribute = c.Attribute("lastBuildStatus")
                     where attribute != null
                     where attribute.Value.Equals(status)
                     let xAttribute = c.Attribute("name")
                     where xAttribute != null
                     let xAttribute1 = c.Attribute("activity")
                     where xAttribute1 != null
                     let attribute1 = c.Attribute("lastBuildLabel")
                     where attribute1 != null
                     let xAttribute2 = c.Attribute("lastBuildTime")
                     where xAttribute2 != null
                     let attribute2 = c.Attribute("webUrl")
                     where attribute2 != null
                     select
                         new Project(xAttribute.Value,
                                     attribute.Value,
                                     xAttribute1.Value,
                                     attribute1.Value,
                                     xAttribute2.Value,
                                     attribute2.Value)
                    );
            return x.ToList();
        }

        private XDocument GetRemoteData()
        {
            return XDocument.Load(new StringReader(this._endPoint.GetXml()));
        }
    }
}