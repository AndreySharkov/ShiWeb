using System;
using System.Collections.Generic;
using System.Text;
using WebServer.Server.HTTP;

namespace WebServer.Server.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string HTML)
            : base(HTML, ContentType.Html)
        {
        }
    }
}