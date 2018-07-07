using System;

namespace CefAdapter
{
    public class BrowserWindowEventArgs : EventArgs
    {
        public BrowserWindowEventArgs(BrowserWindow browserWindow)
        {
            BrowserWindow = browserWindow;
        }

        public BrowserWindow BrowserWindow { get; }
    }
}
