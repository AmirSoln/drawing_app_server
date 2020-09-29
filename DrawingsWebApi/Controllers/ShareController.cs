using DIContracts.Dto;
using DrawingContracts.Dto.Sharing;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class ShareController : ControllerBase
    {
        private readonly ISharingService _sharingService;

        public ShareController(ISharingService sharingService)
        {
            _sharingService = sharingService;
        }

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response GetAllUsersForSharing([FromBody] GetAllUsersForSharingRequest request)
        {
            return _sharingService.GetAllUsersForSharing(request);
        }

        //[Route("api/[controller]/[action]")]
        //[HttpPost]
        //public Response GetSharedUserOfDocument([FromBody] SharedUsersOfDocumentRequest request)
        //{
        //    return _sharingService.GetSharedUsersOfDocument(request);
        //}

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response SharedDocument([FromBody] ShareDocumentRequest request)
        {
            return _sharingService.ShareDocument(request);
        }

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response GetSharedDocuments([FromBody] GetSharedDocumentsRequest request)
        {
            return _sharingService.GetSharedDocument(request);
        }

        // PUT api/<ShareController>/5
        //[Route("api/[controller]")]
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<ShareController>/5
        //[Route("api/[controller]")]
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
