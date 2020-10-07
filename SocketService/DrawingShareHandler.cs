using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DrawingContracts.Dto.SocketDto;

namespace SocketService
{
    public class DrawingShareHandler : WebSocketHandler
    {
        public DrawingShareHandler(SocketManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task<bool> OnConnected(string docId, string userId, WebSocket socket)
        {
            var isConnected = await base.OnConnected(docId, userId, socket);
            if (isConnected)
            {
                var userInfo = new UserInfo { email = userId };
                var sentData = new BaseDto("Connected") { Data = userInfo };
                await SendMessageToAllAsync(docId, JsonSerializer.Serialize(sentData));
                await SendCurrentlyLoggedInUsers(docId, userId, socket);
            }

            return true;
        }

        private async Task SendCurrentlyLoggedInUsers(string docId, string userId, WebSocket socket)
        {
            //get all users of the document
            var docSockets = WebSocketManager.GetAll(docId);
            if (docSockets != null)
            {
                //filter myself from result
                var users = docSockets.Keys.Where(user => user != userId).Select(key => new UserInfo() { email = key });

                //build result
                var getUsers = new GetCurrentOnlineUsers(users);
                var sentData = new BaseDto("GetUsers") { Data = getUsers };

                //send to the right socket
                await SendMessageAsync(socket, JsonSerializer.Serialize(sentData));
            }
        }

        public override async Task OnDisconnected(string docId, string userId)
        {
            await base.OnDisconnected(docId, userId);
            var userInfo = new UserInfo { email = userId };
            var sentData = new BaseDto("Disconnected") { Data = userInfo };
            await SendMessageToAllAsync(docId, JsonSerializer.Serialize(sentData));
        }


        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer, string docId)
        {
            Console.WriteLine($"{Encoding.UTF8.GetString(buffer, 0, result.Count)}");
            var sentData = new BaseDto("Notification");
            await SendMessageToAllAsync(docId, JsonSerializer.Serialize(sentData));
        }
    }
}
