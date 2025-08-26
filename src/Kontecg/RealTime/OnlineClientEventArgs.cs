using System;

namespace Kontecg.RealTime
{
    public class OnlineClientEventArgs : EventArgs
    {
        public OnlineClientEventArgs(IOnlineClient client)
        {
            Client = client;
        }

        public IOnlineClient Client { get; }
    }
}
