using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Server.HTTP_Request
{
    public enum StatusCode
    {
        OK = 200,
        Found = 302,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404
    }
}
