using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpService _signUpService;

        public SignUpController(ISignUpService signUpService)
        {
            _signUpService = signUpService;
        }

        // POST api/<SignUpController>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public SignUpResponse SignUp([FromBody] SignUpRequest request)
        {
            return _signUpService.SignUp(request);
        }

        // PUT api/<SignUpController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<SignUpController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
