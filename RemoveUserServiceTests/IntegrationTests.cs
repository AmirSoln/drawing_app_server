using System;
using System.Collections.Generic;
using System.Text;
using DrawingContracts.Dto;
using DrawingContracts.Dto.RemoveUser;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using DrawingDal;
using InfraDal;
using InfraDalContracts;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RemoveUserService;
using SignUpService;

namespace RemoveUserServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private InfraDalImpl _infraDal;
        private IDbConnection _connection;
        private IRemoveUserService _removeUserService;

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
            var dummyData = CreateDummyData(dataCount);

            //when we try to remove a user with non existing user id (mark as removed)
            var request = new RemoveUserRequest() { UserId = id };
            var result = _removeUserService.RemoveUser(request);

            //then we get RemoveUserNoUserFoundResponse
            Assert.That(result, Is.TypeOf(expectedType));

            DestroyDummyData(dummyData);
        }

        [Test]
        public void TestRemoveExistingUser_GetRemoveResponseOkAndSuccess()
        {
            const int dataCount = 5;
            var expectedType = typeof(RemoveUserResponseOk);
            var random = new Random();

            //given a db with some users
            var dummyData = CreateDummyData(dataCount);

            //when we try to remove a user with non existing user id (mark as removed)
            var request = new RemoveUserRequest() { UserId = dummyData[random.Next(dataCount)] };
            var result = _removeUserService.RemoveUser(request);

            //then we get RemoveUserResponseOk
            //and that user is marked as deleted
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(GetUserById(request.UserId), Is.False);

            });

            DestroyDummyData(dummyData);
        }

        private bool GetUserById(string requestUserId)
        {
            var dataSet = _infraDal.ExecuteQuery(_connection,
                        "SELECT ACTIVE FROM USERS WHERE USER_ID ='" + requestUserId + "'");
            var data = dataSet.Tables[0].Rows[0][0];
            return Convert.ToBoolean(data);
        }

        private List<string> CreateDummyData(int dataCount)
        {
            var retval = new List<string>();
            for (int i = 0; i < dataCount; i++)
            {
                var userResponse =
                    _signUpService.SignUp(new SignUpRequest { Login = new LoginDto { Email = "DUMMY.MAIL@DUMMY" + i, Username = "DUMMY" + i } });
                retval.Add(((SignUpResponseOk)userResponse).SignUpRequest.Login.Email);
            }

            return retval;
        }

        private void DestroyDummyData(List<string> ids)
        {
            var query = new StringBuilder();
            query.Append("DELETE FROM ").Append("USERS ").Append("WHERE USERS.USER_ID IN (");
            foreach (var id in ids)
            {
                query.Append("'").Append(id).Append("'").Append(",");
            }

            query.Length--;
            query.Append(")");
            _infraDal.ExecuteQuery(_connection, query.ToString());
        }
    }
}