namespace SqlToGraphite.Plugin.CCTray.UnitTests.CCTray
{
    using System;

    using NUnit.Framework;

    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_PipelineLengths
    {
        private IEndPoint _endPoint;

        private IDateTimeNow dateTimeNow;

        [SetUp]
        public void SetUp()
        {
            this._endPoint = MockRepository.GenerateMock<IEndPoint>();
            this.dateTimeNow = MockRepository.GenerateMock<IDateTimeNow>();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [Test]
        public void Should_Load_cctray_and_return_single_pipeline_last_build_length_in_seconds()
        {
            int seconds = 59;
            var past = new DateTime(2012, 11, 10, 9, 8, seconds);
            dateTimeNow.Expect(x => x.GetNow()).Return(new DateTime(2012, 11, 10, 9, 8, 0));
            var xml = string.Format("<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='{0}' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /></Projects>", GetPastDateTime(past));
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            ccTray.Load();
            this._endPoint.VerifyAllExpectations();
            var lengths = ccTray.GetPipelineLengths();
            Assert.That(lengths[0].Name, Is.EqualTo("Create_Environment"));
            Assert.That(lengths[0].Length, Is.EqualTo(seconds));
        }

        [Test]
        public void Should_Load_cctray_and_return_single_pipeline_multiple_stages_last_build_length_in_seconds()
        {
            int seconds = 59;
            var past = new DateTime(2012, 11, 10, 9, 8, seconds);
            var past = new DateTime(2012, 11, 10, 9, 8, seconds);
            dateTimeNow.Expect(x => x.GetNow()).Return(new DateTime(2012, 11, 10, 9, 8, 0));
            var xml = string.Format("<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='{0}' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' />Project name='Create_Environment :: SomthingElse' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='{1}' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /></Projects>", GetPastDateTime(past), GetPastDateTime(past));
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            ccTray.Load();
            this._endPoint.VerifyAllExpectations();
            var lengths = ccTray.GetPipelineLengths();
            Assert.That(lengths[0].Name, Is.EqualTo("Create_Environment"));
            Assert.That(lengths[0].Length, Is.EqualTo(seconds));
        }

        private string GetPastDateTime(DateTime when)
        {
            var date = when.ToString("yyyy-MM-dd");
            var time = when.ToString("hh:mm:ss");
            var value = string.Format("{0}T{1}", date, time);
            return value;
        }
    }
}