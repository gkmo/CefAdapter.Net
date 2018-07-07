using System;

namespace CefAdapter
{    
    class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string name, string[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }

        public string[] Arguments { get; }
    }
}