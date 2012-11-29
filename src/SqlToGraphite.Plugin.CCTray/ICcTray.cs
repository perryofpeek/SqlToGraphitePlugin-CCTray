namespace SqlToGraphite.Plugin.CCTray
{
    using System.Collections.Generic;

    public interface ICcTray
    {
        List<Project> FailedPipelines();
        List<Project> AllPipelines();
        List<string> AllPipelineNames();
        void Load();
    }
}