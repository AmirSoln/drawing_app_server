using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DrawingContracts.Interface
{
    public interface ISocketManager
    {
        bool AddConnection(string docId, string userId, WebSocket webSocket);
        Task RemoveConnection(string docId, string userId);
        WebSocket GetSocket(string docId, string userId);
        ConcurrentDictionary<string, WebSocket> GetAll(string docId);
    }
}
