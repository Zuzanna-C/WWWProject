using System.Net.Sockets;
using System.Net;
using System.Text;

namespace CoolFan.HelpClasses
{
    public class ConnectArduino
    {
        public string _arduinoIp;
        private static int _commandPort = 4568;
        private UdpClient client;
        private string command = "initialize";

        public ConnectArduino()
        {
            client = new UdpClient(0);
            establishconnection();
        }

        public async void establishconnection()
        {
            using (client)
            {
                if (_arduinoIp == null) { _arduinoIp = "255.255.255.255"; };
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_arduinoIp), _commandPort);

                byte[] data = Encoding.UTF8.GetBytes(command);

                await client.SendAsync(data, data.Length, endPoint);

                client.Client.ReceiveTimeout = 5000;

                try
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string response = Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"Received response: {response}");
                    _arduinoIp = response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving response: {ex.Message}");
                }
            }
        }
    }
}
