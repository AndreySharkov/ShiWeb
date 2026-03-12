using System.Text;
using System.Web;
using WebServer.Server;
using WebServer.Server.HTTP;
using WebServer.Server.HTTP_Request;
using WebServer.Server.Responses;
using WebServer.Server.Views;

namespace WebServer.demo
{
    public class StartUp
    {
        private const string TargetUser = "user";
        private const string TargetPass = "user123";

        public static async Task Main()
        {
            string[] sitesToScrape = { "https://frogonaquest.com/", "https://google.com/" };
            await ExportWebDataAsync(Form.FileName, sitesToScrape);

            var webServer = new HttpServer(endpoints =>
            {
                endpoints
                    .MapGet("/", new TextResponse("Hello from the updated server!"))
                    .MapGet("/Home/About", new HtmlResponse("<h1>About</h1><p>Use this area to provide additional information.</p>"))
                    .MapGet("/Home/Numbers", new HtmlResponse("", ShowNumbersTo50))
                    .MapGet("/Home/NumbersToN", new HtmlResponse("", ShowNumbersToN))
                    .MapPost("/Home/NumbersToN", new HtmlResponse("", ShowNumbersToN))
                    .MapGet("/Products/My-Products", new HtmlResponse("", ShowAllProducts))
                    .MapGet("/Products/AllAsJson", new HtmlResponse("", ShowAllProductsAsJson))
                    .MapGet("/Products/AllAsText", new TextResponse("", ShowAllProductsAsText))
                    .MapGet("/Products/AllAsTextFile", new TextFileResponse("products.txt"))
                    .MapGet("/HTML", new HtmlResponse("<h1>HTML response</h1>"))
                    .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                    .MapGet("/TestNameAge", new HtmlResponse(Form.HTML))
                    .MapPost("/TestNameAge", new TextResponse("", ParseFormInput))
                    .MapGet("/Content", new HtmlResponse(Form.DownloadForm))
                    .MapPost("/Content", new TextFileResponse(Form.FileName))
                    .MapGet("/Cookies", new HtmlResponse("", EvaluateCookies))
                    .MapGet("/Session", new TextResponse("", CheckSessionDetails))
                    .MapGet("/Login", new HtmlResponse(LoginPage.LoginForm))
                    .MapPost("/Login", new HtmlResponse("", PerformLogin))
                    .MapGet("/Logout", new HtmlResponse("", PerformLogout))
                    .MapGet("/UserProfile", new HtmlResponse("", ShowProfile))
                    .MapGet("/Chat/Show", new HtmlResponse("", ShowChat))
                    .MapPost("/Chat/Send", new RedirectResponse("/Chat/Show", SendChatMessage));
            });

            await webServer.Start();
        }

        private static readonly List<KeyValuePair<string, string>> ChatMessages = new();

        private static void ShowChat(Request req, Response res)
        {
            var sb = new StringBuilder();
            sb.Append("<h3>Messages:</h3>");
            if (ChatMessages.Any())
            {
                foreach (var message in ChatMessages)
                {
                    sb.Append($@"<div class='card .bg-light col-6'>
                        <div class='card-body'>
                            <blockquote class='blockquote mb-0'>
                                <p>{message.Value}</p>
                                <footer class='blockquote-footer'>{message.Key}</footer>
                            </blockquote>
                        </div>
                    </div>");
                }
            }
            else
            {
                sb.Append("<p>No messages yet!</p>");
            }

            sb.Append(@"<p></p>
            <form action='/Chat/Send' method='post'>
                <div class='form-group card-header row'>
                    <div class='col-12'>
                        <h5>Send a new message</h5>
                    </div>
                    <div class='col-8'>
                        <label>Message: </label>
                        <textarea name='Message' class='form-control' rows='3'></textarea>
                    </div>
                    <div class='col-4'>
                        <label>Sender Name: </label>
                        <input name='Sender' class='form-control'>
                        <input class='btn btn-primary mt-2 float-lg-right' type='submit' value='Send' />
                    </div>
                </div>
            </form>");
            res.Body = sb.ToString();
        }

        private static void SendChatMessage(Request req, Response res)
        {
            if (req.FromData.ContainsKey("Sender") && req.FromData.ContainsKey("Message"))
            {
                var sender = req.FromData["Sender"];
                var message = req.FromData["Message"];
                ChatMessages.Add(new KeyValuePair<string, string>(sender, message));
            }
        }

        private static void ShowNumbersTo50(Request req, Response res)
        {
            var sb = new StringBuilder();
            sb.Append("<h2>Nums 1 ... 50</h2><ul>");
            for (int i = 1; i <= 50; i++)
            {
                sb.Append($"<li>{i}</li>");
            }
            sb.Append("</ul>");
            res.Body = sb.ToString();
        }

        private static void ShowNumbersToN(Request req, Response res)
        {
            int count = 3;
            if (req.FromData.ContainsKey("count"))
            {
                int.TryParse(req.FromData["count"], out count);
            }

            var sb = new StringBuilder();
            sb.Append($"<h2>Nums 1 ... {count}</h2><ul>");
            for (int i = 1; i <= count; i++)
            {
                sb.Append($"<li>{i}</li>");
            }
            sb.Append("</ul>");
            sb.Append(@"<form method='POST'>
                <input name='count' value='" + count + @"'>
                <button type='submit'>Submit</button>
            </form>");
            res.Body = sb.ToString();
        }

        private static readonly dynamic[] Products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 1500.00 },
            new { Id = 2, Name = "Mouse", Price = 25.50 },
            new { Id = 3, Name = "Keyboard", Price = 50.00 }
        };

        private static void ShowAllProducts(Request req, Response res)
        {
            var sb = new StringBuilder();
            sb.Append("<h1>All Products</h1><ul>");
            foreach (var p in Products)
            {
                sb.Append($"<li>N.{p.Id}: {p.Name} - {p.Price} lv.</li>");
            }
            sb.Append("</ul>");
            res.Body = sb.ToString();
        }

        private static void ShowAllProductsAsJson(Request req, Response res)
        {
            var json = "[" + string.Join(",", Products.Select(p => $"{{\"Id\":{p.Id},\"Name\":\"{p.Name}\",\"Price\":{p.Price}}}")) + "]";
            res.Body = json;
            res.Headers.Add(Header.ContentType, "application/json");
        }

        private static void ShowAllProductsAsText(Request req, Response res)
        {
            var text = string.Join(Environment.NewLine, Products.Select(p => $"Product {p.Id}: {p.Name} - {p.Price} lv."));
            res.Body = text;
            File.WriteAllText("products.txt", text);
        }

        private static void ParseFormInput(Request req, Response res)
        {
            res.Body = string.Join(
                Environment.NewLine, 
                req.FromData.Select(kvp => $"{kvp.Key} - {kvp.Value}")
            ) + Environment.NewLine;
        }

        private static async Task<string> FetchPageSourceAsync(string targetUrl)
        {
            using var client = new HttpClient();
            var pageContent = await client.GetStringAsync(targetUrl);
            
            return pageContent.Length > 2000 ? pageContent[..2000] : pageContent;
        }

        private static async Task ExportWebDataAsync(string path, string[] targetUrls)
        {
            var fetchTasks = targetUrls.Select(FetchPageSourceAsync);
            var results = await Task.WhenAll(fetchTasks);

            string separator = Environment.NewLine + new string('-', 100);
            await File.WriteAllTextAsync(path, string.Join(separator, results));
        }

        private static void EvaluateCookies(Request req, Response res)
        {
            bool hasActiveCookies = req.Cookies.Any(c => c.Name != Session.SessionCookieName);

            if (!hasActiveCookies)
            {
                res.Cookies.Add("My-Cookie", "My-Value");
                res.Cookies.Add("My-Second-Cookie", "My-Second-Value");
                res.Body = "<h1>Cookies set!</h1>";
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<h1>Cookies</h1><table border='1'><tr><th>Name</th><th>Value</th></tr>");
            
            foreach (var cookie in req.Cookies)
            {
                sb.Append($"<tr><td>{HttpUtility.HtmlEncode(cookie.Name)}</td><td>{HttpUtility.HtmlEncode(cookie.Value)}</td></tr>");
            }
            sb.Append("</table>");
            
            res.Body = sb.ToString();
        }

        private static void CheckSessionDetails(Request req, Response res)
        {
            bool isSessionActive = req.Session.ContainsKey(Session.SessionCurrentDateKey);

            res.Body = isSessionActive 
                ? $"Stored data: {req.Session[Session.SessionCurrentDateKey]}!" 
                : "Current date stored!";
        }

        private static void PerformLogin(Request req, Response res)
        {
            req.Session.Clear();

            if (req.FromData.ContainsKey("Username") && req.FromData.ContainsKey("Password"))
            {
                bool isValidUser = req.FromData["Username"] == TargetUser;
                bool isValidPass = req.FromData["Password"] == TargetPass;

                if (isValidUser && isValidPass)
                {
                    req.Session[Session.SessionUserKey] = "MyUserId";
                    res.Cookies.Add(Session.SessionCookieName, req.Session.Id);
                    res.Body = "<h3>Logged successfully! </h3>";
                    return;
                }
            }
            
            res.Body = LoginPage.LoginForm;
        }

        private static void PerformLogout(Request req, Response res)
        {
            req.Session.Clear();
            res.Body = "<h3>Logged out successfully!</h3>";
        }

        private static void ShowProfile(Request req, Response res)
        {
            res.Body = req.Session.ContainsKey(Session.SessionUserKey)
                ? $"<h3>Currently logged-in user is with username '{TargetUser}'</h3>"
                : "<h3>You should first log in - <a href='/Login'>Login</a></h3>";
        }
    }
}
