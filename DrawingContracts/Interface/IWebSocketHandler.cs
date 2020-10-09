using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DrawingContracts.Interface
{
    public interface IWebSocketHandler
    {
        Task<bool> OnConnected(string docId, string userId, WebSocket socket);
        Task OnDisconnected(string docId, string userId);
        Task SendMessageAsync(WebSocket socket, string message);
        Task SendMessageToAllAsync(string docId, string message);
        Task SendMessageAsync(string docId, string userId, string message);
        Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer,string docId);
    }
}
