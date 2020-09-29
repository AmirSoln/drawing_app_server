using System.Collections.Generic;
using DIContracts.Dto;

namespace DrawingContracts.Dto.Sharing
{
    public class GetAllUsersForSharingResponseOk:Response
    {
        public IEnumerable<SharingUserInfo> Users { get; set; }
    }
}
