using System;
using System.Collections.Generic;
using System.Text;

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
