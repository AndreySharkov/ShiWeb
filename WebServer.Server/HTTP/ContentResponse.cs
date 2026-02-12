
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using WebServer.Server.Common;
using WebServer.Server.HTTP_Request;

namespace WebServer.Server.HTTP
{
    public class ContentResponse : Response
    {
        public ContentResponse(string content, string contentType)
            : base(StatusCode.OK)
        {
            Guard.AgainstNull(content);
            Guard.AgainstNull(contentType);

            this.Headers.Add(Header.ContentType, contentType);
            this.Body = content;
        }

        public override string ToString()
        {
            if (this.Body != null)
            {
                var contentLength = Encoding.UTF8.GetByteCount(this.Body).ToString();
                this.Headers.Add(Header.ContentLength, contentLength);

            }
            return base.ToString();
        }
    }
}
