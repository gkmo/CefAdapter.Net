using System.Threading;
using System;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

namespace CefAdapter
{
    class InterProcessCommunicator
    {
        private readonly RequestSocket _requestSocket;
        private readonly ResponseSocket _replySocket;
        private ManualResetEvent _connectedResetEvent;
        private Thread _thread;
        private bool _stop;        

        public InterProcessCommunicator()
        {
            _requestSocket = new RequestSocket("tcp://localhost:5560");
            _replySocket = new ResponseSocket("tcp://localhost:5561");            
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        public bool IsConnected { get; private set; }

        public bool Connect()
        {
            _stop = false;
            
            _thread = new Thread(ListenRequests);
            _thread.IsBackground = true;
            _thread.Start();    

            _connectedResetEvent = new ManualResetEvent(false);

            _requestSocket.Connect("tcp://localhost:5560");

            var reply = Invoke("CONNECT");

            IsConnected = reply == "CONNECT|SUCCESS";

            return IsConnected;
        }

        internal void ExecuteJavaScript(int id, string code)
        {
            Invoke("EXECUTE_JAVA_SCRIPT", id, code);
        }

        internal void ShowDeveloperTools(int id)
        {
            Invoke("SHOW_DEVELOPER_TOOLS", id);
        }

        public void Disconnect()
        {
            IsConnected = false;
            _stop = true;
        }

        private void ListenRequests(object obj)
        {
            while(!_stop)
            {
                var message = ReceiveMessage(_replySocket);
                var result = ProcessMessage(message);
                SendMessage(_replySocket, result);
            }
        }

        private string ProcessMessage(string message)
        {
            var splittedMessage = message.Split('|');
            var messageName = splittedMessage[0];
            var arguments = new string[splittedMessage.Length - 1];

            Array.Copy(splittedMessage, 1, arguments, 0, arguments.Length);

            Task.Run(() => MessageReceived?.Invoke(this, new MessageEventArgs(messageName, arguments)));

            return splittedMessage[0] + "|SUCCESS";
        }

        internal void OnQuerySuccess(long id, string message)
        {
            Invoke("QUERY_SUCCESS", id, message);
        }

        private string Invoke(string eventName, params object[] arguments)
        {
            SendMessage(_requestSocket, eventName, arguments);
            return ReceiveMessage(_requestSocket);
        }

        internal void OnQueryFailure(long queryId, int errorCode, string message)
        {
            Invoke("QUERY_FAILURE", queryId, errorCode, message);
        }

        private static string ReceiveMessage(IReceivingSocket socket)
        {            
            var message = socket.ReceiveFrameString();
            Console.WriteLine("Received: {0}", message);
            return message;
        }

        private static void SendMessage(IOutgoingSocket socket, string eventName, params object[] arguments)
        {
            var message = eventName + "|" + string.Join("|", arguments);            
            socket.SendFrame(message);
            Console.WriteLine("Sent: {0}", message);
        }
    }
}   