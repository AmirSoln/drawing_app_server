using DIContracts.Dto;
using DrawingContracts.Dto.Documents;

namespace DrawingContracts.Interface
{
    public interface IDocumentService
    {
        Response UploadDocument(string documentName, string filePath, string userId);
        Response GetDocumentById(string id);
        Response DeleteDocumentById(DeleteDocumentRequest request);
        Response GetAllDocuments(string owner);
    }
}
