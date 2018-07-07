using CefAdapter;
using System;
using System.Threading;

namespace CefAdapter.Samples.Simple
{
    class Program
    {
        private static Thread _timerThread;
        private static bool _stopTimer;

        static void Main(string[] args)
        {
            var application = new Application(@"../../../src/presentation/index.html");

            application.BrowserWindowCreated += OnBrowserWindowCreated;

            application.Run();        
        }

        private static void OnBrowserWindowCreated(object sender, BrowserWindowEventArgs e)
        {
            e.BrowserWindow.ContextCreated += StartTimer;
            e.BrowserWindow.Closing += StopTimer;            
            e.BrowserWindow.On("showDeveloperTools", ProcessOpenDeveloperToolsRequest);                    
        }

        private static void ProcessOpenDeveloperToolsRequest(JavaScriptRequest request)
        {
            request.BrowserWindow.ShowDeveloperTools();
            request.Success("Request processed in C#!");
        }

        private static void StartTimer(object sender, BrowserContextCreatedEventArgs e)
        {
            if (_timerThread != null)
            {
                return;
            }            

            _stopTimer = false;
            _timerThread = new Thread(Timer);
            _timerThread.Start(e.BrowserWindow);            
        }

        private static void StopTimer(object sender, BrowserWindowEventArgs e)
        {            
            _stopTimer = true;
            _timerThread = null;
        }

        private static void Timer(object state)
        {
            var browser = (BrowserWindow) state;            

            while (!_stopTimer)
            {
                browser.Send("timer", DateTime.Now);
                Thread.Sleep(1000);    
            }
        }
    }
}
