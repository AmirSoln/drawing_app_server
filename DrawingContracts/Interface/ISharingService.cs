using DIContracts.Dto;
using DrawingContracts.Dto.Sharing;

namespace DrawingContracts.Interface
{
    public interface ISharingService
    {
        Response GetSharedDocument(GetSharedDocumentsRequest request);
        Response ShareDocument(ShareDocumentRequest request);
        Response GetAllUsersForSharing(GetAllUsersForSharingRequest request);
        //Response GetSharedUsersOfDocument(SharedUsersOfDocumentRequest request);
    }
}
