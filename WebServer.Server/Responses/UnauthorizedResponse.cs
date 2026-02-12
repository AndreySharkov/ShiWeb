using System;
using System.Collections.Generic;
using System.Text;
using WebServer.Server.HTTP_Request;

namespace WebServer.Server.Responses
{
    public class UnauthorizedResponse : Response
    {
        public UnauthorizedResponse()
            : base(StatusCode.Unauthorized)
        {
        }
    }
}