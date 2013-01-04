namespace SqlToGraphite.Plugin.CCTray
{
    public class ProjectLengths
    {
        public ProjectLengths(string name, int length)
        {
            Name = name;
            Length = length;
        }

        public string Name { get; set; }

        public int Length { get; set; }
    }
}