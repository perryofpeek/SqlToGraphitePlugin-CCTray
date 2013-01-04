namespace SqlToGraphite.Plugin.CCTray
{
    using System;

    public class DateTimeNowImpl : IDateTimeNow
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }
    }
}