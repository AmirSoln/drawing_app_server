namespace DrawingContracts.Dto.SignIn
{
    public class SignInResponseOK:SignInResponse
    {
        public SignInRequest Request { get; }

        public SignInResponseOK(SignInRequest request)
        {
            Request = request;
        }
    }
}
