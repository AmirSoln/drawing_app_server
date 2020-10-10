using System.Collections.Generic;
using System.Text;
using DrawingContracts.Dto;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using DrawingDal;
using InfraDal;
using InfraDalContracts;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SignUpService;

namespace SignUpServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private InfraDalImpl _infraDal;
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
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _drawingDal = null;
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
            var dummyData = CreateDummyData(dataCount);

            //when we try to signup new user with valid data (not existing in db)
            var login = new LoginDto {Email = email,Username = username};
            var request = new SignUpRequest {Login = login};
            var result = _signUpService.SignUp(request);

            //we get SignUpResponseOk
            Assert.That(result,Is.TypeOf(expectedType));

            dummyData.Add(email);
            DestroyDummyData(dummyData.ToArray());
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
            var dummyData = CreateDummyData(dataCount);

            //when we try to signup new user with valid data (not existing in db)
            var login = new LoginDto {Email = email,Username = username};
            var request = new SignUpRequest {Login = login};
            var result = _signUpService.SignUp(request);

            //we get SignUpResponseOk
            Assert.That(result,Is.TypeOf(expectedType));

            dummyData.Add(email);
            DestroyDummyData(dummyData.ToArray());
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

        private void DestroyDummyData(params string[] ids)
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