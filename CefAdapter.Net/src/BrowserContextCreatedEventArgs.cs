using System;
using System.Collections.Generic;
using System.Text;

namespace CefAdapter
{
    public class BrowserContextCreatedEventArgs : BrowserWindowEventArgs
    {
        public BrowserContextCreatedEventArgs(BrowserWindow browserWindow, int frameId) : base(browserWindow)
        {            
            FrameId = frameId;
        }        

        public int FrameId { get; }
    }
}
