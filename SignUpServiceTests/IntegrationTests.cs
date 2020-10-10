using DrawingContracts.Dto;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using DrawingDal;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SignUpService;
using TestingUtilities;

namespace SignUpServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private TestUtilitiesImpl _testUtilitiesImpl;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _testUtilitiesImpl = new TestUtilitiesImpl(strConn);
            _drawingDal = new DrawingDalImpl(_configuration);
        }

        [SetUp]
        public void Setup()
        {
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _signUpService = null;
        }

        [TestCase("gogo@gmail.com","momo")]
        [TestCase("gogo@nana10.com","gogo")]
        [TestCase("gogo@hotmail.com","soso")]
        [TestCase("gogo@walla.com","lolo")]
        public void TestSignUpWithValidUsernameAndEmail(string email,string username)
        {
            var expectedType = typeof(SignUpResponseOk);
            const int dataCount = 5;
            //given a database with some data
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we try to signup new user with valid data (not existing in db)
            var login = new LoginDto {Email = email,Username = username};
            var request = new SignUpRequest {Login = login};
            var result = _signUpService.SignUp(request);

            //we get SignUpResponseOk
            Assert.That(result,Is.TypeOf(expectedType));

            dummyData.Add(email);
            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }

        [TestCase("DUMMY.MAIL@DUMMY0", "DUMMY1")]
        [TestCase("DUMMY.MAIL@DUMMY1", "DUMMY2")]
        [TestCase("", "DUMMY1")]
        [TestCase(null, "DUMMY3")]
        public void TestSignUpWithInvalidEmail(string email,string username)
        {
            var expectedType = typeof(SignUpResponseInvalidCredentials);
            const int dataCount = 5;
            //given a database with some data
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we try to signup new user with valid data (not existing in db)
            var login = new LoginDto {Email = email,Username = username};
            var request = new SignUpRequest {Login = login};
            var result = _signUpService.SignUp(request);

            //we get SignUpResponseOk
            Assert.That(result,Is.TypeOf(expectedType));

            dummyData.Add(email);
            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }
    }
}