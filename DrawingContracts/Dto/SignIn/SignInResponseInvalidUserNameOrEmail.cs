namespace DrawingContracts.Dto.SignIn
{
    public class SignInResponseInvalidUserNameOrEmail:SignInResponse
    {
        public SignInRequest Request { get; }

        public SignInResponseInvalidUserNameOrEmail(SignInRequest request)
        {
            Request = request;
        }
    }
}
