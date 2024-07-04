using CoolFan.Models;

namespace CoolFan.Interfaces
{
    public interface ISensorDataFetcher
    {
        public Task fetchData();
        public Task<SensorData> getSensorDataAsync();
    }
}
