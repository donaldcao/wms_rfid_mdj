using SignalR;
using THOK.Common.SignalR;

namespace THOK.Wms.SignalR.Connection
{
    public class AutomotiveSystemsConnection : PersistentConnection
    {
    }
    public class AutomotiveSystemsNotify
    {
        public static void Notify()
        {
            (new Notifier<AutomotiveSystemsConnection>()).Notify("TaskStart");
        }
    }
}
