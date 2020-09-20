namespace DrawingContracts.Dto.SignUp
{
    public class SignUpResponseOk:SignUpResponse
    {
        public SignUpRequest SignUpRequest { get; set; }

        public SignUpResponseOk(SignUpRequest signUpRequest)
        {
            SignUpRequest = signUpRequest;
        }
    }
}
