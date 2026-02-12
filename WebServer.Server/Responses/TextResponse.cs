using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebServer.Server.HTTP;

namespace WebServer.Server.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string text)
            : base(text, ContentType.PlainText)
        {
        }
    }
}