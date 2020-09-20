﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DrawingContracts.Dto.RemoveUser
{
    public class RemoveUserNoUserFoundResponse:RemoveUserResponse
    {
        public RemoveUserNoUserFoundResponse(RemoveUserRequest request)
        {
            Request = request;
        }

        public RemoveUserRequest Request { get; set; }
    }
}
