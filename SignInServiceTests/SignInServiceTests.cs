using NUnit.Framework;

namespace SignInServiceTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        //TODO - fix tests
        //[Test]
        //public void TestSignInWithExistingUserAndGetResponseOk()
        //{
        //    var signInService = new SignInServiceImpl(new DrawingDalImpl());
        //    var request = new SignInRequest();
        //    var login = new LoginDTO { Email = "saksuk23@gmail.com" };
        //    request.LoginDto = login;
        //    var response = signInService.SignIn(request);
        //    Assert.IsInstanceOf(typeof(SignInResponseOK),response);
        //}

        //[Test]
        //public void TestSignInWithNonExistingUserAndGetResponseInvalidCredentials()
        //{
        //    var signInService = new SignInServiceImpl(new DrawingDalImpl());
        //    var request = new SignInRequest();
        //    var login = new LoginDTO { Email = "sakasdasdsuk23124@gmail.com" };
        //    request.LoginDto = login;
        //    var response = signInService.SignIn(request);
        //    Assert.IsInstanceOf(typeof(SignInResponseInvalidUserNameOrEmail),response);
        //}
    }
}