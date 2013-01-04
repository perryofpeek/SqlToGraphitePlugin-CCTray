namespace SqlToGraphite.Plugin.CCTray.UnitTests.CCTray
{
    using NUnit.Framework;

    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming
    [TestFixture]    
    public class With_SuccessPipelines
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
        public void Should_Load_cctray_and_return_success_list_with_two_item()
        {
            var xml = "<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='SomePipeline :: Create_VMs :: Create_Master_SQL_Server' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-04T17:02:27' webUrl='http://build.london.ttldev.local:8153/go/tab/build/detail/Create_Environment/1/Create_VMs/1/Create_Master_SQL_Server' /></Projects>";
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            ccTray.Load();
            this._endPoint.VerifyAllExpectations();
            Assert.That(ccTray.SuccessPipelines()[0].PipelineName, Is.EqualTo("Create_Environment"));
            Assert.That(ccTray.SuccessPipelines()[1].PipelineName, Is.EqualTo("SomePipeline"));
        }

        [Test]
        public void Should_Load_cctray_and_return_success_list_with_zero_items()
        {
            var xml = "<Projects><Project name='Create_Environment :: Create_VMs' activity='Sleeping' lastBuildStatus='Failure' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-18T14:18:06' webUrl='http://build.london.ttldev.local:8153/go/pipelines/Create_Environment/1/Create_VMs/1' /><Project name='SomePipeline :: Create_VMs :: Create_Master_SQL_Server' activity='Sleeping' lastBuildStatus='Failure' lastBuildLabel='Environment_Number_1' lastBuildTime='2011-08-04T17:02:27' webUrl='http://build.london.ttldev.local:8153/go/tab/build/detail/Create_Environment/1/Create_VMs/1/Create_Master_SQL_Server' /></Projects>";
            this._endPoint.Expect(x => x.GetXml()).Return(xml);
            var ccTray = new CcTray(this._endPoint, dateTimeNow);
            ccTray.Load();
            this._endPoint.VerifyAllExpectations();
            Assert.That(ccTray.SuccessPipelines().Count, Is.EqualTo(0));
        }
    }    
}