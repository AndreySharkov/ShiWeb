using System.Net;
using System.Net.Sockets;

namespace Class1.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;

        private readonly int port;

        private readonly TcpListener serverListener;

        public HttpServer(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.serverListener = new TcpListener(this.ipAddress, this.port);
        }

        public void Start()
        {
            this.serverListener.Start();
            Console.WriteLine($"Server started on port {this.port}");
            Console.WriteLine("Listening for requests");
            while (true)
            {
                var connection = this.serverListener.AcceptTcpClient();
                var networkStream = connection.GetStream();
                string content = "Hello from the server!";
                int contentLength = System.Text.Encoding.UTF8.GetByteCount(content);
                var response = $@"HTTP/1.1 Content-Type:text/html; charset=UTF-8 Content-Length: {contentLength} {content}";
                var responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
                networkStream.Write(responseBytes);
                connection.Close();
            }
        }
    }
}
