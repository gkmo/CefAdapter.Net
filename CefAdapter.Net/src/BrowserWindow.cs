
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CefAdapter
{
    public class BrowserWindow
    {
        private readonly int _id;
        private readonly InterProcessCommunicator _nativeInterface;
        private readonly Dictionary<string, Action<JavaScriptRequest>> _javaScriptQueryHandlers;

        private bool _isClosing;

        internal BrowserWindow(int id, InterProcessCommunicator nativeInterface)
        {
            _id = id;
            _nativeInterface = nativeInterface;
            _javaScriptQueryHandlers = new Dictionary<string, Action<JavaScriptRequest>>();
        }

        public event EventHandler<BrowserContextCreatedEventArgs> ContextCreated;

        public event EventHandler<BrowserWindowEventArgs> Closing;

        public void Send(string channel, object argument)
        {
            if (!_isClosing)
            {            
                var jsonArgument = JsonConvert.SerializeObject(argument);

                _nativeInterface.ExecuteJavaScript(_id, $"ipcDotNet.receive('{channel}', '{jsonArgument}');");
            }
        }

        public void ShowDeveloperTools()
        {
            _nativeInterface.ShowDeveloperTools(_id);
        }

        public void On(string channel, Action<JavaScriptRequest> handler)
        {
            _javaScriptQueryHandlers[channel] = handler;
        }

        internal void OnContextCreated(int frameId)
        {
            ContextCreated?.Invoke(this, new BrowserContextCreatedEventArgs(this, frameId));
        }

        internal void OnClosing()
        {
            _isClosing = true;
            Closing?.Invoke(this, new BrowserWindowEventArgs(this));
        }

        internal bool ProcessJavaScriptRequest(int frameId, long queryId, string request, 
            JavaScriptRequestSuccessCallback successCallback, JavaScriptRequestFailureCallback failureCallback)
        {
            var jsRequest = JsonConvert.DeserializeObject<JavaScriptRequest>(request);
            
            if (!_javaScriptQueryHandlers.TryGetValue(jsRequest.Channel, out var handler))
            {
                return false;
            }
            
            jsRequest.FrameId = frameId;
            jsRequest.QueryId = queryId;
            jsRequest.SuccessCallback = successCallback;
            jsRequest.FailureCallback = failureCallback;
            jsRequest.BrowserWindow = this;
            
            handler(jsRequest);

            return true;
            // if (jsRequest.Channel == "openDevTools")
            // {
            //     if (_nativeInterface.ShowDeveloperTools(browserId))
            //     {
            //         successCallback(queryId, "Opened developer tools");
            //     }
            //     else
            //     {
            //          failureCallback(queryId, 1, "Failed to open developer tools");
            //     }

            //     return true;
            // }            
        }
    }
}