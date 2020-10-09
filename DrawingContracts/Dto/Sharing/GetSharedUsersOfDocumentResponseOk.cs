using System.Collections.Generic;

namespace DrawingContracts.Dto.Sharing
{
    public class GetSharedUsersOfDocumentResponseOk:GetSharedUsersOfDocumentResponse
    {
        public GetSharedUsersOfDocumentResponseOk(SharedUsersOfDocumentRequest request) : base(request)
        {
        }

        public IEnumerable<SharingUserInfo> Users { get; set; }
    }
}
