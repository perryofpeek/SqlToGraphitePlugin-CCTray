namespace SqlToGraphite.Plugin.CCTray.UnitTests.endPoint
{
    using NUnit.Framework;

    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GetXml
    {

        private IHttpGet _httpGet;

        [SetUp]
        public void SetUp()
        {
            this._httpGet = MockRepository.GenerateMock<IHttpGet>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Should_get_cctray_item_success()
        {
            var responseXml = "<someXml/>";
            var url = "http://some.uri.com/page.xml";
            this._httpGet.Expect(x => x.Request(url));
            this._httpGet.Expect(x => x.StatusCode).Return(200);
            this._httpGet.Expect(x => x.ResponseBody).Return(responseXml);
            var endpoint = new EndpointImpl(this._httpGet, url);
            var xml = endpoint.GetXml();
            Assert.That(xml, Is.EqualTo(responseXml));
        }

        [Test]
        public void Should_throw_exception_if_status_is_not_200_OK()
        {
            var responseXml = "<someXml/>";
            var url = "http://some.uri.com/page.xml";
            this._httpGet.Expect(x => x.Request(url));
            this._httpGet.Expect(x => x.StatusCode).Return(404);
            this._httpGet.Expect(x => x.ResponseBody).Return(responseXml);
            var endpoint = new EndpointImpl(this._httpGet, url);
            var ex = Assert.Throws<HttpException>(() => endpoint.GetXml());
            Assert.That(ex.Message, Is.EqualTo("Http status code is 404"));
        }
    }
}