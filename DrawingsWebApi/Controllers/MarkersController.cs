using DIContracts.Dto;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class MarkersController : ControllerBase
    {
        private IMarkerService _markerService;

        public MarkersController(IMarkerService markerService)
        {
            _markerService = markerService;
        }

        // GET api/<MarkersController>/GetAllMarkers
        [Route("api/[controller]/[action]/{docId}")]
        [HttpGet]
        public Response GetMarkers(string docId)
        {
            return _markerService.GetAllMarkers(docId);
        }

        // POST api/<MarkersController>/CreateMarker
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response CreateMarker([FromBody] CreateMarkerRequest request)
        {
            return _markerService.CreateMarker(request);
        }

        // POST api/<MarkersController>/DeleteMarker
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response DeleteMarker([FromBody] DeleteMarkerRequest request)
        {
            return _markerService.DeleteMarker(request);
        }

        // PUT api/<MarkersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<MarkersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
