using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace WebServer.Server.HTTP_Request
{
    public class Response
    {
        public StatusCode StatusCode { get; init; }
        public HeaderCollection Headers { get; } = new HeaderCollection();
        public string Body { get; set; }
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers.Add(Header.Server, "My Web Server");
            this.Headers.Add(Header.Data, $"{DateTime.UtcNow:r}");
        }
        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");
            foreach (var header in this.Headers)
            {
                result.AppendLine(header.ToString());
            }
            result.AppendLine();
            if (!string.IsNullOrEmpty(this.Body))
            {
                result.AppendLine(this.Body);
            }
            return result.ToString();
        }
    }
}