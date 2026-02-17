using System.Runtime.CompilerServices;
using WebServer.Server;
using WebServer.Server.HTTP_Request;
using WebServer.Server.Responses;

namespace WebServer.demo
{
    public class StartUp
    {
        public static void Main()
        {
            var server = new HttpServer(routes =>
            {
                routes
                .MapGet("/", new TextResponse("Hello from the server!"))
                .MapGet("/HTML", new HtmlResponse("<h1>HTML response</h1>"))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/login", new HtmlResponse(WebServer.Server.Form.HTML))
                .MapPost("/login", new TextResponse("", AddFormDataAction));
                
            });
            server.Start();
        }
        private static void AddFormDataAction(
            Request request, Response response)
        {
            response.Body = "";

            foreach(var (key, value) in request.FromData)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }
    }
}
