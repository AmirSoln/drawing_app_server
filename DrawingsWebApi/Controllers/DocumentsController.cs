using System;
using System.IO;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DrawingsWebApi.Controllers
{
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Route("api/[controller]/[action]/{id}")]
        [HttpGet]
        public GetDocumentResponse GetDocumentById(string id)
        {
            return _documentService.GetDocumentById(id);
        }


        // POST api/<DocumentsController>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public Response Upload()
        {
            var httpRequest = HttpContext.Request;
            //Upload Image
            var postedFile = httpRequest.Form.Files["Image"];

            //Create custom filename
            httpRequest.Form.TryGetValue("Name", out var temp);
            httpRequest.Form.TryGetValue("UserId", out var userId);
            var strUserId = userId.ToString().Trim();
            var documentName = temp + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", documentName);
            var response = _documentService.UploadDocument(documentName, filePath, strUserId);
            if (response is UploadDocumentResponseOk)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
            }

            return response;
        }

        [Route(("api/[controller]/[action]"))]
        [HttpPost]
        public Response DeleteDocumentById([FromBody] DeleteDocumentRequest request)
        {
            return _documentService.DeleteDocumentById(request);
        }

        // PUT api/<DocumentsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<DocumentsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
