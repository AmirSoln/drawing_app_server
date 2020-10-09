using System;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.RemoveUser;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Interface;

namespace RemoveUserService
{
    [Register(Policy.Transient, typeof(IRemoveUserService))]
    public class RemoveUserServiceImpl : IRemoveUserService
    {
        private readonly IDrawingDal _drawingDal;

        public RemoveUserServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public Response RemoveUser(RemoveUserRequest request)
        {
            Response response = new RemoveUserResponseOk(request);
            var user = _drawingDal.GetUser(new SignInRequest() { LoginDto = new LoginDto() { Email = request.UserId } });
            if (user.Tables[0].Rows.Count == 0)
            {
                response = new RemoveUserNoUserFoundResponse(request);
            }
            try
            {
                _drawingDal.RemoveUser(request.UserId);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }
    }
}
