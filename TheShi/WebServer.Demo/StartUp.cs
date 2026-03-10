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
                    .MapGet("/", new TextResponse("Hello from the server!"))
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
                    .MapGet("/UserProfile", new HtmlResponse("", ShowProfile));
            });

            await webServer.Start();
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

            bool isValidUser = req.FromData["Username"] == TargetUser;
            bool isValidPass = req.FromData["Password"] == TargetPass;

            if (isValidUser && isValidPass)
            {
                req.Session[Session.SessionUserKey] = "MyUserId";
                res.Cookies.Add(Session.SessionCookieName, req.Session.Id);
                res.Body = "<h3>Logged successfully! </h3>";
            }
            else
            {
                res.Body = LoginPage.LoginForm;
            }
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