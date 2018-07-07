using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using CefAdapter;

namespace Angular
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            
            webHost.Start();

            var application = new Application(@"http://localhost:5000");

            application.BrowserWindowCreated += OnBrowserWindowCreated;

            application.Run();

            webHost.StopAsync();
            webHost.WaitForShutdown();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void OnBrowserWindowCreated(object sender, BrowserWindowEventArgs e)
        {
            e.BrowserWindow.On("showDeveloperTools", ProcessOpenDeveloperToolsRequest);                    
        }

        private static void ProcessOpenDeveloperToolsRequest(JavaScriptRequest request)
        {
            request.BrowserWindow.ShowDeveloperTools();
            request.Success("Request processed in C#!");
        }
    }
}
