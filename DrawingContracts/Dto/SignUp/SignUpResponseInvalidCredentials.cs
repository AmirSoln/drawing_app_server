namespace DrawingContracts.Dto.SignUp
{
    public class SignUpResponseInvalidCredentials:SignUpResponse
    {
        public SignUpRequest SignUpRequest { get; set; }

        public SignUpResponseInvalidCredentials(SignUpRequest signUpRequest)
        {
            SignUpRequest = signUpRequest;
        }
    }
}
