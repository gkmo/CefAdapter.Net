

namespace CefAdapter
{
    public class JavaScriptRequest
    {
        public string Channel { get; set; }

        public object Argument { get; set; }

        public BrowserWindow BrowserWindow { get; set; }

        public int FrameId { get; internal set; }

        internal long QueryId { get; set; }

        internal JavaScriptRequestSuccessCallback SuccessCallback { get; set; }

        internal JavaScriptRequestFailureCallback FailureCallback { get; set; }

        public void Success(string message)
        {
            SuccessCallback(QueryId, message);
        }

        public void Failure(int errorCode, string message)
        {
            FailureCallback(QueryId, errorCode, message);
        }
    }
}