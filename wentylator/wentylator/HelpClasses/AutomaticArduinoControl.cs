using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoolFan.HelpClasses
{
    public class AutomaticArduinoControl
    {
        private static int _port = 4567;
        private ConnectArduino _connectArduino = new ConnectArduino();

        public async Task SetTresholdOn(float threshold)
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                string message = $"setTresholdOn {threshold}";
                byte[] data = Encoding.UTF8.GetBytes(message);

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

        public async Task SetTresholdOff(float threshold)
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                string message = $"setTresholdOff {threshold}";
                byte[] data = Encoding.UTF8.GetBytes(message);

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

        public async Task setAutoModeOn()
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                byte[] data = Encoding.UTF8.GetBytes("autoModeOn");

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

        public async Task setAutoModeOff()
        {
            string arduinoIP = _connectArduino._arduinoIp;
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _port);

                byte[] data = Encoding.UTF8.GetBytes("autoModeOff");

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
