using DIContracts.Dto;
using DrawingContracts.Dto.SignIn;

namespace DrawingContracts.Interface
{
    public interface ISignInService
    {
        Response SignIn(SignInRequest request);
    }
}
