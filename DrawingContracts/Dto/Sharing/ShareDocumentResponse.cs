using DIContracts.Dto;

namespace DrawingContracts.Dto.Sharing
{
    public class ShareDocumentResponse:Response
    {
        public ShareDocumentResponse(ShareDocumentRequest request)
        {
            Request = request;
        }

        public ShareDocumentRequest Request { get; set; }

    }
}
