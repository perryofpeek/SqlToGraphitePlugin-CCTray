namespace SqlToGraphite.Plugin.CCTray.UnitTests
{
    using log4net;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SqlToGraphiteInterfaces;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_CCTrayClient
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=Master;User Id=sa;Password=!bcde1234;";

        private const string SimpleQuery = "SELECT 234 , DATEADD(day,11,GETDATE())";

        private const string SimplePath = "Some.Path";

        private IEncryption encryption;

        private ILog log;

        private Encryption e;

        [SetUp]
        public void SetUp()
        {
            this.e = new Encryption();
            this.encryption = MockRepository.GenerateMock<IEncryption>();
            this.log = MockRepository.GenerateMock<ILog>();
            this.encryption.Expect(x => x.Decrypt(this.e.Encrypt(ConnectionString))).Return(ConnectionString);
        }

        [Test]
        public void Should_get_result()
        {          
        }

        [Test]
        public void Should_get_result_with_date()
        {           
        }

        [Test]
        public void Should_get_result_with_date_and_name_set_in_select()
        {           
        }

        [Test]
        public void Should_get_results()
        {          
        }
    }
}