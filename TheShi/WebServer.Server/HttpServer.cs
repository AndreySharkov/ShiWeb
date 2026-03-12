using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebServer.Server.Contracts;
using WebServer.Server.HTTP;
using WebServer.Server.HTTP_Request;

namespace WebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress hostAddress;
        private readonly int serverPort;
        private readonly TcpListener listener;
        private readonly RoutingTable router;

        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingConfig)
        {
            hostAddress = IPAddress.Parse(ipAddress);
            serverPort = port;
            listener = new TcpListener(hostAddress, serverPort);
            
            routingConfig(router = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routes)
            : this("127.0.0.1", port, routes)
        {
        }

        public HttpServer(Action<IRoutingTable> routes)
            : this(8080, routes)
        {
        }

        public async Task Start()
        {
            listener.Start();

            Console.WriteLine($"Server started on port {serverPort}");
            Console.WriteLine("Listening for requests ... ");
            
            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => ProcessClientAsync(tcpClient));
            }
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                var rawRequest = await ExtractRequestAsync(stream);
                
                Console.WriteLine(rawRequest);
                
                var parsedRequest = Request.Parse(rawRequest);
                var matchedResponse = router.MatchRequest(parsedRequest);

                matchedResponse.PreRenderAction?.Invoke(parsedRequest, matchedResponse);
                
                ManageSessionState(parsedRequest, matchedResponse);
                
                await SendResponseAsync(stream, matchedResponse);
            }
        }

        private static void ManageSessionState(Request req, Response res)
        {
            if (!req.Session.ContainsKey(Session.SessionCurrentDateKey))
            {
                req.Session[Session.SessionCurrentDateKey] = DateTime.Now.ToString();
                res.Cookies.Add(Session.SessionCookieName, req.Session.Id);
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, Response res)
        {
            var payload = Encoding.UTF8.GetBytes(res.ToString());
            await stream.WriteAsync(payload);
        }

        private async Task<string> ExtractRequestAsync(NetworkStream stream)
        {
            int bufferSize = 1024;
            byte[] dataBuffer = new byte[bufferSize];
            var requestContent = new StringBuilder();

            while (true)
            {
                int currentBytes = await stream.ReadAsync(dataBuffer, 0, bufferSize);
                if (currentBytes == 0) break;

                requestContent.Append(Encoding.UTF8.GetString(dataBuffer, 0, currentBytes));

                if (currentBytes < bufferSize || !stream.DataAvailable)
                {
                    // Basic check to see if we have the full headers
                    if (requestContent.ToString().Contains("\r\n\r\n"))
                    {
                        break;
                    }
                }
            }

            return requestContent.ToString();
        }
    }
}