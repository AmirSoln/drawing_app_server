using System.Data;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Dto.SignUp;

namespace DrawingContracts.Interface
{
    public interface IDrawingDal
    {
        bool CreateUser(SignUpRequest request);
        DataSet GetUser(SignInRequest request);
        public void RemoveUser(string userId);
        bool UploadDocument(string docId, string userId, string filePath, string documentName);
        DataSet GetDocumentById(string id);
        void DeleteDocument(DeleteDocumentRequest request);
        void CreateMarker(CreateMarkerRequest request);
        void DeleteMarker(string markerId);
        DataSet GetAllMarkers(string documentId);
    }
}
