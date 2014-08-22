using SignalR;
using THOK.Common.SignalR.Model;
using System;
using System.Threading;

namespace THOK.Common.SignalR
{
    public class Notifier<TPersistentConnection> where TPersistentConnection : PersistentConnection
    {
        public string ConnectionId { get; set; }
        public ProgressState ProgressState { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public void Notify(object message)
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<TPersistentConnection>();
            context.Connection.Broadcast(message);
        }

        public void NotifyGroup(string group, object message)
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<TPersistentConnection>();
            context.Groups.Send(group, message);
        }

        public void NotifyConnection(string connectionId, object message)
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<TPersistentConnection>();
            context.Connection.Send(connectionId, message);
        }

        public void NotifyConnection(object message)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetConnectionContext<TPersistentConnection>();
                context.Connection.Send(ConnectionId, message);
            }
            catch (Exception)
            {
            }
        }

        public void NotifyMessage()
        {
            NotifyConnection(ProgressState.Clone());
        }

        public void NotifyProgress(string totalName, int totalValue, string currentName, int currentValue)
        {
            ProgressState.State = StateType.Processing;
            ProgressState.TotalProgressName = totalName;
            ProgressState.TotalProgressValue = totalValue;
            ProgressState.CurrentProgressName = currentName;
            ProgressState.CurrentProgressValue = currentValue;
            NotifyConnection(ProgressState.Clone());
        }
    }
}
