using System.Net.Sockets;

namespace DrawWithLineUs.Con.Service
{
    public interface ICommunicationService
    {
        void ConnectToLineUs(out TcpClient client, out NetworkStream stream, string lineusIP, int lineusport);
        void Transmit(NetworkStream stream, string instruction);
    }
}