using System.Net.Sockets;
using System.Net;
using System.Text;
using CoolFan.Interfaces;

namespace CoolFan.HelpClasses
{
    public class FanControlService : IFanControlService
    {
        private static int _port = 4567;
        private ConnectArduino _connectArduino = new ConnectArduino();

        public async Task turnON()
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                byte[] data = Encoding.UTF8.GetBytes("on");

                await client.SendAsync(data, data.Length, endPoint);

                client.Client.ReceiveTimeout = 5000;

                try
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string response = Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"Received response: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving response: {ex.Message}");
                }
            }
        }

        public async Task turnOFF()
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                byte[] data = Encoding.UTF8.GetBytes("off");

                await client.SendAsync(data, data.Length, endPoint);

                client.Client.ReceiveTimeout = 5000;

                try
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string response = Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"Received response: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving response: {ex.Message}");
                }
            }
        }
    }
}
