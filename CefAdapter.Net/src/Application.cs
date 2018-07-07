using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace CefAdapter
{
    public class Application
    {
        private readonly Dictionary<int, BrowserWindow> _browserWindows = new Dictionary<int, BrowserWindow>();
        private readonly InterProcessCommunicator _interProcessCommunicator;
        private readonly string _initialUrl;

        public Application(string initialPage)
        {
            _interProcessCommunicator = new InterProcessCommunicator();
            _interProcessCommunicator.MessageReceived += OnMessageReceived;

            if (!initialPage.StartsWith("http://") && !initialPage.StartsWith("https://"))
            {
                var rootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                initialPage = string.Format("file:///{0}", Path.GetFullPath(Path.Combine(rootDirectory, initialPage)));
            }

            _initialUrl = initialPage;
       }

        public event EventHandler<BrowserWindowEventArgs> BrowserWindowCreated;        

        public BrowserWindow MainBrowserWindow { get; private set; }        

        public void Run()
        {
            var exeExtension = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeExtension = ".exe";
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo("CefAdapter.Browser" + exeExtension, $"--url={_initialUrl}")
            };

            if (process.Start())
            {
                _interProcessCommunicator.Connect();                
            }

            process.WaitForExit();
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.Name)
            {
                case "ON_BROWSER_CREATED":
                    OnBrowserCreated(int.Parse(e.Arguments[0]));
                    break;
                case "ON_BROWSER_CLOSED":
                    OnBrowserClosed(int.Parse(e.Arguments[0]));
                    break;
                case "ON_CONTEXT_CREATED":
                    OnContextCreated(int.Parse(e.Arguments[0]), int.Parse(e.Arguments[1]));
                    break;
                case "ON_QUERY":
                    {
                        OnQuery(int.Parse(e.Arguments[0]), int.Parse(e.Arguments[1]), long.Parse(e.Arguments[2]), e.Arguments[3], 
                            _interProcessCommunicator.OnQuerySuccess, _interProcessCommunicator.OnQueryFailure);
                    }                    
                    break;
            }
        }

        private void OnBrowserCreated(int browserId)
        {
            var browserWindow = new BrowserWindow(browserId, _interProcessCommunicator);

            _browserWindows[browserId] = browserWindow;

            if (MainBrowserWindow == null)
            {
                MainBrowserWindow = browserWindow;
            }

            BrowserWindowCreated?.Invoke(this, new BrowserWindowEventArgs(browserWindow));
        }   

        private void OnBrowserClosed(int browserId)
        {
            if (_browserWindows.TryGetValue(browserId, out var browserWindow))
            {                
                browserWindow.OnClosing();

                if (MainBrowserWindow == browserWindow)
                {
                    MainBrowserWindow = null;
                }

                _browserWindows.Remove(browserId);
            }                    
        }   

        private void OnContextCreated(int browserId, int frameId)
        {
            if (_browserWindows.TryGetValue(browserId, out var browserWindow))
            {
                browserWindow.OnContextCreated(frameId);
            }
        }        

        private bool OnQuery(int browserId, int frameId, long queryId, string request, 
            JavaScriptRequestSuccessCallback successCallback, JavaScriptRequestFailureCallback failureCallback)
        {            
            Console.WriteLine($"OnQuery {queryId} - {request}");
            
            if (_browserWindows.TryGetValue(browserId, out var browserWindow))
            {
                return browserWindow.ProcessJavaScriptRequest(frameId, queryId, request, successCallback, failureCallback);
            }

            return false;
        }
    }
}