using System;
using System.Collections.Generic;
using System.Text;

namespace DrawingContracts.Dto.RemoveUser
{
    public class RemoveUserResponseOk:RemoveUserResponse
    {
        public RemoveUserResponseOk(RemoveUserRequest request)
        {
            Request = request;
        }

        public RemoveUserRequest Request { get; set; }
    }
}
