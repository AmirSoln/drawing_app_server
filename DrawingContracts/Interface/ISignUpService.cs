using DrawingContracts.Dto.SignUp;

namespace DrawingContracts.Interface
{
    public interface ISignUpService
    {
        SignUpResponse SignUp(SignUpRequest request); 
    }
}
