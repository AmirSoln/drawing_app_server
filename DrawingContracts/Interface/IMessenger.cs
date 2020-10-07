using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DrawingContracts.Interface
{
    public interface IMessenger
    {
        Task Send(string id, string message);
        IReceiver Add(string id,WebSocket socket);
    }
}
