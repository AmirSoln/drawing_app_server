using System.Collections.Generic;

namespace DrawingContracts.Dto.SocketDto
{
    public class GetCurrentOnlineUsers
    {
        public GetCurrentOnlineUsers(IEnumerable<UserInfo> users)
        {
            Users = users;
        }

        public IEnumerable<UserInfo> Users { get; set; }
    }
}
