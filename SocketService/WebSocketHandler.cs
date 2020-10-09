using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DrawingContracts.Interface;

namespace SocketService
{
    public abstract class WebSocketHandler:IWebSocketHandler
    {
        protected ISocketManager WebSocketManager { get; set; }

        public WebSocketHandler(ISocketManager webSocketConnectionManager)
        {
            WebSocketManager = webSocketConnectionManager;
        }

        public virtual async Task<bool> OnConnected(string docId, string userId, WebSocket socket)
        {
            return WebSocketManager.AddConnection(docId, userId, socket);
        }

        public virtual async Task OnDisconnected(string docId, string userId)
        {
            await WebSocketManager.RemoveConnection(docId, userId);
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                    offset: 0,
                    count: message.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string docId, string userId, string message)
        {
            await SendMessageAsync(WebSocketManager.GetSocket(docId, userId), message);
        }

        public async Task SendMessageToAllAsync(string docId, string message)
        {
            var webSockets = WebSocketManager.GetAll(docId);
            if (webSockets != null)
            {
                foreach (var (key, value) in webSockets)
                {
                    if (value.State == WebSocketState.Open)
                        await SendMessageAsync(value, message);
                }
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer,string docId);
    }
}
