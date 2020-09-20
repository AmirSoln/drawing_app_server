using System;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Interface;

namespace SignInService
{
    [Register(Policy.Transient,typeof(ISignInService))]
    public class SignInServiceImpl:ISignInService
    {
        private readonly IDrawingDal _drawingDal;

        public SignInServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public Response SignIn(SignInRequest request)
        {
            try
            {
                var ds  = _drawingDal.GetUser(request);
                var tbl = ds.Tables[0];
                SignInResponse retval = new SignInResponseInvalidUserNameOrEmail(request);
                if (tbl.Rows.Count == 1)
                {
                    if (request.LoginDto.Email==(string) tbl.Rows[0][0])
                    {
                        retval = new SignInResponseOK(request);
                    }
                   
                }
                return retval;
            }
            catch(Exception ex)
            {
                return new AppResponseError(ex.Message); 
            }
        }
    }
}
