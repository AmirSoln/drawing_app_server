using DIContracts.Dto;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ISignInService _signInService;

        public SignInController(ISignInService signInService)
        {
            _signInService = signInService;
        }

        // POST api/<SignInController>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response SignIn([FromBody] SignInRequest request)
        {
            return _signInService.SignIn(request);
        }

        // PUT api/<SignInController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<SignInController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
