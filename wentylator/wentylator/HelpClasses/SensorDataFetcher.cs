using System.Net.Sockets;
using System.Net;
using System.Text;
using CoolFan.Models;
using CoolFan.Interfaces;
using System.Globalization;

namespace CoolFan.HelpClasses
{
    public class SensorDataFetcher : ISensorDataFetcher
    {
        private static int _arduinoPort = 4567; // Stały port nasłuchu na Arduino
        private UdpClient client;
        private SensorData _sensorData = new SensorData();
        private string command = "data";
        private ConnectArduino _connectArduino = new ConnectArduino();

        public SensorDataFetcher()
        {
            // Dynamicznie przydzielany port przez system operacyjny
            client = new UdpClient(0);
        }

        public async Task<SensorData> getSensorDataAsync()
        {
            await fetchData();
            return _sensorData;
        }

        public async Task fetchData()
        {
            string arduinoIP = _connectArduino._arduinoIp;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), _arduinoPort);

            byte[] data = Encoding.UTF8.GetBytes(command);

            await client.SendAsync(data, data.Length, endPoint);

            client.Client.ReceiveTimeout = 5000;

            try
            {
                UdpReceiveResult result = await client.ReceiveAsync();
                string response = Encoding.UTF8.GetString(result.Buffer);

                string[] parts = response.Split(',');

                if (parts.Length == 2)
                {
                    float temperature = float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat);
                    float humidity = float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat);
                    _sensorData = new SensorData
                    {
                        Temperature = temperature,
                        Humidity = humidity
                    };
                    //Console.WriteLine($"Temperature: {_sensorData.Temperature}, Humidity: {_sensorData.Humidity}");
                }
                else
                {
                    Console.WriteLine("Failed to parse sensor data.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving response: {ex.Message}");
            }
        }
    }
}
