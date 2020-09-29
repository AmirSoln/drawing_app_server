using DIContracts.Dto;

namespace DrawingContracts.Dto.Sharing
{
    public class GetSharedUsersOfDocumentResponse:Response
    {
        public GetSharedUsersOfDocumentResponse(SharedUsersOfDocumentRequest request)
        {
            Request = request;
        }

        public SharedUsersOfDocumentRequest Request { get; set; }
    }

}
