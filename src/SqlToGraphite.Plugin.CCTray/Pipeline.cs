namespace SqlToGraphite.Plugin.CCTray
{
    using System.Collections.Generic;

    public class Pipeline
    {
        public string Name { get; set; }

        public IList<string> Users { get; set; }
    }
}