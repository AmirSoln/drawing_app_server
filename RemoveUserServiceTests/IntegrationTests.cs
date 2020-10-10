using System;
using DrawingContracts.Dto.RemoveUser;
using DrawingContracts.Interface;
using DrawingDal;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RemoveUserService;
using SignUpService;
using TestingUtilities;

namespace RemoveUserServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private IRemoveUserService _removeUserService;
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
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [SetUp]
        public void Setup()
        {
            _removeUserService = new RemoveUserServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _removeUserService = null;
        }

        [TestCase("Moshe.mail@gmasdjjs.com")]
        [TestCase("Yossdad.com")]
        [TestCase("")]
        [TestCase(null)]
        public void TestRemoveNonExistingUser_GetRemoveUserNoUserFoundResponse(string id)
        {
            const int dataCount = 5;
            var expectedType = typeof(RemoveUserNoUserFoundResponse);

            //given a db with some users
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we try to remove a user with non existing user id (mark as removed)
            var request = new RemoveUserRequest() { UserId = id };
            var result = _removeUserService.RemoveUser(request);

            //then we get RemoveUserNoUserFoundResponse
            Assert.That(result, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }

        [Test]
        public void TestRemoveExistingUser_GetRemoveResponseOkAndSuccess()
        {
            const int dataCount = 5;
            var expectedType = typeof(RemoveUserResponseOk);
            var random = new Random();

            //given a db with some users
            var dummyData = _testUtilitiesImpl.CreateUserDummyData(_signUpService,dataCount);

            //when we try to remove a user with non existing user id (mark as removed)
            var request = new RemoveUserRequest() { UserId = dummyData[random.Next(dataCount)] };
            var result = _removeUserService.RemoveUser(request);

            //then we get RemoveUserResponseOk
            //and that user is marked as deleted
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(_testUtilitiesImpl.GetUserById(request.UserId), Is.False);

            });

            _testUtilitiesImpl.DestroyUserDummyData(dummyData.ToArray());
        }
    }
}