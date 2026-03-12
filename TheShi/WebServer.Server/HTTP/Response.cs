using System.Text;
using WebServer.Server.HTTP;

namespace WebServer.Server.HTTP_Request
{
    public class Response
    {
        public StatusCode StatusCode { get; init; }
        public HeaderCollection Headers { get; } = new HeaderCollection();
        public CookieCollection Cookies { get; } = new CookieCollection();
        public string? Body { get; set; }
        public Action<Request, Response>? PreRenderAction { get; protected set; }
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers.Add(Header.Server, "My Web Server");
            this.Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}\r\n");
            foreach (var header in this.Headers)
            {
                result.Append($"{header}\r\n");
            }
            foreach(var cookie in Cookies)
            {
                result.Append($"{Header.SetCookie}: {cookie}\r\n");
            }
            result.Append("\r\n");
            if (!string.IsNullOrWhiteSpace(this.Body))
            {
                result.Append(this.Body);
            }
            return result.ToString();
        }
    }
}
