using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Text;

namespace TheFirstWeb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");

            int port = 8080;
            var serverListener = new TcpListener(ipAddress, port);
            serverListener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Listening for requests");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();
                var networkStream = connection.GetStream();

                string content = "Hello from the server!";
                int contentLength = Encoding.UTF8.GetByteCount(content);

                var response = $@"HTTP/1.1 Content-Type:text/html; charset=UTF-8 Content-Length: {contentLength} {content}";

                var responseBytes = Encoding.UTF8.GetBytes(response);
                networkStream.Write(responseBytes);
                connection.Close();
            }
        }
    }
}