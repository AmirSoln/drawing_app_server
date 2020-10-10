using System;
using System.Collections.Generic;
using System.Text;
using DrawingContracts.Dto;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using DrawingDal;
using InfraDal;
using InfraDalContracts;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SignInService;
using SignUpService;

namespace SignInServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private ISignInService _signInService;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private InfraDalImpl _infraDal = new InfraDalImpl();
        private IDbConnection _connection;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _infraDal = new InfraDalImpl();
            _connection = _infraDal.Connect(strConn);
        }

        [SetUp]
        public void Setup()
        {
            _drawingDal = new DrawingDalImpl(_configuration);
            _signInService = new SignInServiceImpl(_drawingDal);
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [Test]
        public void TestSignInWithExistingUserAndGetResponseOk()
        {
            var dataCount = 5;
            var random = new Random();
            var expectedType = typeof(SignInResponseOK);

            //given a database with at least dataCount records
            var dummyData = CreateDummyData(dataCount);

            //when we login with an existing id
            var request = new SignInRequest();
            var login = new LoginDto { Email = dummyData[random.Next(dataCount)] };
            request.LoginDto = login;
            var response = _signInService.SignIn(request);

            //then we get SignInResponseOK
            Assert.That(response, Is.TypeOf(expectedType));

            DestroyDummyData(dummyData);
        }

        [TestCase("momo@sdadadd")]
        [TestCase("momo")]
        [TestCase("")]
        [TestCase(null)]
        public void TestSignInWithNonExistingUserAndGetResponseInvalidCredentials(string email)
        {
            var dataCount = 5;
            var expectedType = typeof(SignInResponseInvalidUserNameOrEmail);

            //given a database with at least dataCount records
            var dummyData = CreateDummyData(dataCount);

            //when we login with an non existing id
            var request = new SignInRequest();
            var login = new LoginDto { Email = email };
            request.LoginDto = login;
            var response = _signInService.SignIn(request);

            //then we get SignInResponseInvalidUserNameOrEmail
            Assert.That(response, Is.TypeOf(expectedType));

            DestroyDummyData(dummyData);
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
            var queryStr = query.ToString();
            _infraDal.ExecuteQuery(_connection, query.ToString());
        }
    }
}