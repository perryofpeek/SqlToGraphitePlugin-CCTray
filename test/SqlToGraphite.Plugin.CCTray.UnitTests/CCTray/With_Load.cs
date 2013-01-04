namespace SqlToGraphite.Plugin.CCTray.UnitTests.CCTray
{
    using NUnit.Framework;

    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_Load
    {
        private IEndPoint _endPoint;

        private IDateTimeNow dateTimeNow;

        [SetUp]
        public void SetUp()
        {
            this._endPoint = MockRepository.GenerateMock<IEndPoint>();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [Test]
        public void Should_Load_cctray_and_return_list_with_all_items()
        {
            var xml = "<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='SomePipeline :: Create_VMs :: Create_Master_SQL_Server' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-04T17:02:27' webUrl='http://build.london.ttldev.local:8153/go/tab/build/detail/Create_Environment/1/Create_VMs/1/Create_Master_SQL_Server' /></Projects>";
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            ccTray.Load();
            this._endPoint.VerifyAllExpectations();
            Assert.That(ccTray.AllPipelines().Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_Load_cctray_and_return_list_of_all_pipeline_names()
        {
            var xml = "<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='SomePipeline :: Create_VMs :: Create_Master_SQL_Server' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-04T17:02:27' webUrl='http://build.london.ttldev.local:8153/go/tab/build/detail/Create_Environment/1/Create_VMs/1/Create_Master_SQL_Server' /></Projects>";
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            // Test
            ccTray.Load();
            // Assert
            this._endPoint.VerifyAllExpectations();
            Assert.That(ccTray.AllPipelineNames().Count, Is.EqualTo(2));
            Assert.That(ccTray.AllPipelineNames()[0], Is.EqualTo("Create_Environment"));
            Assert.That(ccTray.AllPipelineNames()[1], Is.EqualTo("SomePipeline"));
        }

        [Test]
        public void Should_Load_cctray_and_return_list_of_all_pipeline_names_no_duplicte()
        {
            var xml = "<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='SomePipeline :: Create_VMs :: Create_Master_SQL_Server' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-04T17:02:27' webUrl='http://build.london.ttldev.local:8153/go/tab/build/detail/Create_Environment/1/Create_VMs/1/Create_Master_SQL_Server' /></Projects>";
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            // Test
            ccTray.Load();
            // Assert
            this._endPoint.VerifyAllExpectations();
            Assert.That(ccTray.AllPipelineNames().Count, Is.EqualTo(2));
            Assert.That(ccTray.AllPipelineNames()[0], Is.EqualTo("Create_Environment"));
            Assert.That(ccTray.AllPipelineNames()[1], Is.EqualTo("SomePipeline"));
        }
    }
}