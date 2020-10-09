using System;
using System.Collections;
using System.Data;
using System.IO;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Interface;

namespace DocumentService
{
    [Register(Policy.Transient, typeof(IDocumentService))]
    public class DocumentServiceImpl : IDocumentService
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

        public Response GetDocumentById(string id)
        {
            Response response;
            try
            {
                var document = _drawingDal.GetDocumentById(id);
                if (document.Tables[0].Rows.Count == 1)
                {
                    var row = document.Tables[0].Rows[0];
                    response = new GetDocumentResponseOk(ConvertRowToDocumentObject(row));
                    AddFileToResponse(response);
                }
                else
                {
                    response = new GetDocumentResponseInvalidId();
                }
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        private void AddFileToResponse(Response response)
        {
            var castResponse = (GetDocumentResponseOk)response;
            var bytes = File.ReadAllBytes(castResponse.Doc.DocUrl);
            ((GetDocumentResponseOk)response).Image = bytes;
        }

        public Response DeleteDocumentById(DeleteDocumentRequest request)
        {
            Response response = new DeleteDocumentResponseOk(request);
            try
            {
                var fileName = ConvertRowToDocumentObject(_drawingDal.GetDocumentById(request.DocId).Tables[0].Rows[0]).DocUrl;
                _drawingDal.DeleteDocument(request);
                TryDeletingFile(fileName);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }


            return response;
        }

        private static void TryDeletingFile(string document)
        {
            try
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", document)))
                {
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", document));
                }
                else Console.WriteLine("File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }

        public Response GetAllDocuments(string owner)
        {
            Response response = new GetAllDocumentsResponseOk(owner);
            try
            {
                var results = _drawingDal.GetAllDocuments(owner);
                var allDocuments = results.Tables[0].AsEnumerable()
                    .Select(ConvertRowToDocumentObject);
                ((GetAllDocumentsResponseOk)response).Documents = allDocuments;
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }


            return response;
        }

        private Document ConvertRowToDocumentObject(DataRow row)
        {
            return new Document()
            {
                DocId = (string)row[0],
                DocName = (string)row[1],
                DocUrl = (string)row[2],
                DocOwner = (string)row[3]
            };
        }
    }

}
