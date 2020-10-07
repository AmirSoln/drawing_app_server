using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketService
{
    public class SocketManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _connections;

        public SocketManager()
        {
            _connections = new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();
        }

        public bool AddConnection(string docId, string userId, WebSocket webSocket)
        {
            bool retval = false;
            if (!_connections.ContainsKey(docId))
            {
                var usersToSocket = new ConcurrentDictionary<string, WebSocket>();
                usersToSocket.TryAdd(userId, webSocket);
                retval = _connections.TryAdd(docId, usersToSocket);
            }
            else
            {
                retval = _connections[docId].TryAdd(userId, webSocket);
            }

            return retval;
        }

        public async Task RemoveConnection(string docId, string userId)
        {
            if (_connections.TryGetValue(docId, out var userConcurrentDictionary))
            {
                if (userConcurrentDictionary.TryGetValue(userId, out var socket))
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }

                userConcurrentDictionary.TryRemove(userId, out _);
                if (userConcurrentDictionary.Keys.Count == 0)
                {
                    _connections.TryRemove(docId, out _);
                }
            }
        }

        public WebSocket GetSocket(string docId, string userId)
        {
            WebSocket retval = null;
            if (_connections.TryGetValue(docId, out var userConcurrentDictionary))
            {
                if (userConcurrentDictionary.TryGetValue(userId, out var socket))
                {
                    retval = socket;
                }
            }

            return retval;
        }

        public ConcurrentDictionary<string,WebSocket> GetAll(string docId)
        {
            if (_connections.TryGetValue(docId, out var docSockets))
            {
                return docSockets;
            }

            return null;
        }
    }
}
