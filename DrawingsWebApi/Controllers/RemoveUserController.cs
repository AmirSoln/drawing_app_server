using DIContracts.Dto;
using DrawingContracts.Dto.RemoveUser;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class RemoveUserController : ControllerBase
    {
        private readonly IRemoveUserService _removeUserService;

        public RemoveUserController(IRemoveUserService removeUserService)
        {
            _removeUserService = removeUserService;
        }

        // POST api/<RemoveUserController>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response RemoveUser([FromBody] RemoveUserRequest request)
        {
            return _removeUserService.RemoveUser(request);
        }

        // PUT api/<RemoveUserController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<RemoveUserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
