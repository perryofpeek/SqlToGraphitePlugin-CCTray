namespace SqlToGraphite.Plugin.CCTray
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class CcTray : ICcTray
    {
        private readonly IEndPoint _endPoint;

        private readonly IDateTimeNow dateTimeNow;

        private XDocument _document;

        public List<Project> FailedPipelines()
        {
            return this.GetProjectsWithLastBuildStatus("Failure");
        }

        public List<Project> SuccessPipelines()
        {
            return this.GetProjectsWithLastBuildStatus("Success");
        }

        public List<Project> AllPipelines()
        {
            return this.GetProjects();
        }

        public List<string> AllPipelineNames()
        {
            var rtn = new List<string>();
            foreach (var p in this.GetProjects().Where(p => !rtn.Contains(p.PipelineName)))
            {
                rtn.Add(p.PipelineName);
            }

            rtn.Sort();
            return rtn;
        }

        public CcTray(IEndPoint endPoint, IDateTimeNow dateTimeNow)
        {
            this._endPoint = endPoint;
            this.dateTimeNow = dateTimeNow;
        }

        public void Load()
        {
            this._document = this.GetRemoteData();
        }

        private List<Project> GetProjects()
        {
            var x = from c in this._document.Descendants("Projects").Descendants("Project")
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
                    select new Project(
                        xAttribute.Value,
                        attribute.Value,
                        xAttribute1.Value,
                        attribute1.Value,
                        xAttribute2.Value,
                        attribute2.Value);
            return x.ToList();
        }

        private List<Project> GetProjectsWithLastBuildStatus(string status)
        {
            var x = from c in this._document.Descendants("Projects").Descendants("Project")
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
                    select new Project(
                        xAttribute.Value,
                        attribute.Value,
                        xAttribute1.Value,
                        attribute1.Value,
                        xAttribute2.Value,
                        attribute2.Value);
            return x.ToList();
        }

        private XDocument GetRemoteData()
        {
            return XDocument.Load(new StringReader(this._endPoint.GetXml()));
        }

        private string GetPipelineName(string fullName)
        {
            var parts = fullName.Split(':');
            return parts[0].Trim();
        }

        public IList<ProjectLengths> GetPipelineLengths()
        {
            var rtn = new List<ProjectLengths>();
            foreach (var project in this.GetProjects())
            {
                var now = dateTimeNow.GetNow();
                var then = DateTime.Parse(project.lastBuildTime);
                var length = then.Subtract(now);
                var pipeline = this.GetPipelineName(project.name);
                rtn.Add(new ProjectLengths(pipeline, Convert.ToInt32(length.TotalSeconds)));
            }

            return rtn;
        }
    }
}