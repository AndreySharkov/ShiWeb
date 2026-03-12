using WebServer.Server.HTTP;
using WebServer.Server.HTTP_Request;

namespace WebServer.Server.Responses
{
    public class RedirectResponse : Response
    {
        public RedirectResponse(string location, Action<Request, Response>? preRenderAction = null)
            : base(StatusCode.Found)
        {
            this.Headers.Add(Header.Location, location);
            this.PreRenderAction = preRenderAction;
        }
    }
}
