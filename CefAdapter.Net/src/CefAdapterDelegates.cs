using System;
using System.Runtime.InteropServices;

namespace CefAdapter
{
    internal delegate void OnBrowserCreatedCallback(int browserId);

    internal delegate void OnBrowserClosingCallback(int browserId);

    internal delegate void OnContextCreatedCallback(int browserId, int frameId);

    internal delegate void JavaScriptRequestSuccessCallback(long queryId, string message);

    internal delegate void JavaScriptRequestFailureCallback(long queryId, int errorCode, string message);

    internal delegate bool JavaScriptRequestCallback(int browserId, int frameId, long queryId, string request, JavaScriptRequestSuccessCallback successCallback, JavaScriptRequestFailureCallback failureCallback);

    // internal interface ICefAdapterNativeInterface
    // {
    //     bool CreateApplication(string url, OnBrowserCreatedCallback browserCreatedCallback, OnBrowserClosingCallback browserClosingCallback, 
    //         OnContextCreatedCallback contextCreatedCallback, JavaScriptRequestCallback queryCallback);

    //     void RunMessageLoop();

    //     void Shutdown();

    //     bool ExecuteJavaScript(int browserId, string code);

    //     bool ShowDeveloperTools(int browserId);        
    // }
}