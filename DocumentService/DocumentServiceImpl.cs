using System;
using System.Data;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Interface;

namespace DocumentService
{
    [Register(Policy.Transient,typeof(IDocumentService))]
    public class DocumentServiceImpl:IDocumentService
    {
        private readonly IDrawingDal _drawingDal;

        public DocumentServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public Response UploadDocument(string documentName, string filePath, string userId)
        {
            Response response = new UploadDocumentResponseOk();
            var docId = Guid.NewGuid().ToString();
            try
            {
                _drawingDal.UploadDocument(docId, userId, filePath, documentName);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        public GetDocumentResponse GetDocumentById(string id)
        {
            GetDocumentResponse response;
            var document = _drawingDal.GetDocumentById(id);
            if (document.Tables[0].Rows.Count==1)
            {
                var row = document.Tables[0].Rows;
                response = new GetDocumentResponseOk(ConvertRowToDocumentObject(row));
            }
            else
            {
                response = new GetDocumentResponseInvalidId();
            }

            return response;
        }

        public Response DeleteDocumentById(DeleteDocumentRequest request)
        {
            Response response = new DeleteDocumentResponseOk(request);
            try
            {
                _drawingDal.DeleteDocument(request);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }


            return response;
        }

        private Document ConvertRowToDocumentObject(DataRowCollection row)
        {
            return new Document()
            {
                DocId = (string) row[0][0],
                DocName = (string) row[0][1],
                DocUrl = (string) row[0][2],
                DocOwner = (string) row[0][3]
            };
        }
    }

}
