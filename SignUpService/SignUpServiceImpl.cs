using DIContracts.Attribute;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;

namespace SignUpService
{
    [Register(Policy.Transient,typeof(ISignUpService))]
    public class SignUpServiceImpl : ISignUpService
    {
        private readonly IDrawingDal _drawingDal;

        public SignUpServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public SignUpResponse SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponseOk(request);

            bool result = _drawingDal.CreateUser(request);
            if (!result)
            {
                response = new SignUpResponseInvalidCredentials(request);
            }

            return response;
        }
    }
}
