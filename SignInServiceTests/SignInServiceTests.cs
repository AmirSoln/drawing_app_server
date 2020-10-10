using System;
using DrawingContracts.Dto;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Interface;
using DrawingDal;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SignInService;
using SignUpService;
using TestingUtilities;

namespace SignInServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private ISignInService _signInService;
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
            _drawingDal = new DrawingDalImpl(_configuration);
            _testUtilitiesImpl = new TestUtilitiesImpl(strConn);
        }

        [SetUp]
        public void Setup()
        {
            _signInService = new SignInServiceImpl(_drawingDal);
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _signUpService = null;
            _signInService = null;
        }

        [Test]
        public void TestSignInWithExistingUserAndGetResponseOk()
        {
            var dataCount = 5;
            var random = new Random();
            var expectedType = typeof(SignInResponseOK);

            //given a database with at least dataCount records
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we login with an existing id
            var request = new SignInRequest();
            var login = new LoginDto {Email = dummyData[random.Next(dataCount)]};
            request.LoginDto = login;
            var response = _signInService.SignIn(request);

            //then we get SignInResponseOK
            Assert.That(response, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }

        [TestCase("momo@sdadadd")]
        [TestCase("momo")]
        [TestCase("")]
        [TestCase(null)]
        public void TestSignInWithNonExistingUserAndGetResponseInvalidCredentials(string email)
        {
            const int dataCount = 5;
            var expectedType = typeof(SignInResponseInvalidUserNameOrEmail);

            //given a database with at least dataCount records
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we login with an non existing id
            var request = new SignInRequest();
            var login = new LoginDto {Email = email};
            request.LoginDto = login;
            var response = _signInService.SignIn(request);

            //then we get SignInResponseInvalidUserNameOrEmail
            Assert.That(response, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }

    }
}