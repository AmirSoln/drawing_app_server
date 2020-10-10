using DrawingContracts.Interface;
using DrawingDal;
using InfraDal;
using InfraDalContracts;
using MarkerService;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MarkerServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private InfraDalImpl _infraDal;
        private IDbConnection _connection;
        private IMarkerService _markerService;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _infraDal = new InfraDalImpl();
            _connection = _infraDal.Connect(strConn);
            _drawingDal = new DrawingDalImpl(_configuration);

        }

        [SetUp]
        public void Setup()
        {
            _markerService = new MarkerServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _markerService = null;
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}