using System;
using System.Net;
using System.Net.Sockets;

namespace DrawWithLineUs.Service
{
    public class CommunicationService : ICommunicationService
    {
        public void ConnectToLineUs(out TcpClient client, out NetworkStream stream, string lineusIP, int lineusport)
        {
            IPAddress lineusipaddress = System.Net.IPAddress.Parse(lineusIP);
            client = new TcpClient();
            client.Connect(new IPEndPoint(lineusipaddress, lineusport));
            stream = client.GetStream();
            SayHello(stream);
        }

        private void SayHello(NetworkStream stream)
        {
            Byte[] data = new Byte[256];
            int bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine($"Received: {responseData}");
        }

        public void Transmit(NetworkStream stream, string instruction)
        {
            //Console.WriteLine($"Sent: {instruction}");
            byte[] data = System.Text.Encoding.ASCII.GetBytes(instruction);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];

            int bytes = stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine($"Received: {responseData}");
        }

    }
}
