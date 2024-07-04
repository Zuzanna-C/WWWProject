using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace wentylator.Pages.FanSettings
{
    [Authorize]
    public class FanSettingsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _configFilePath;
        private const string DefaultSensorIp = "192.168.188.253";

        public FanSettingsModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fanconfig.json");
            EnsureConfigFileExists();
        }

        [BindProperty]
        public string SensorIp { get; set; }

        public bool? IsDeviceAvailable { get; set; }

        public void OnGet()
        {
            // Odczytaj aktualne ustawienia czujnika
            var configJson = System.IO.File.ReadAllText(_configFilePath);
            var config = JObject.Parse(configJson);
            SensorIp = config["SensorIp"]?.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Zapisz ustawienia czujnika
            var configJson = System.IO.File.ReadAllText(_configFilePath);
            var config = JObject.Parse(configJson);
            config["SensorIp"] = SensorIp;
            System.IO.File.WriteAllText(_configFilePath, config.ToString());

            // SprawdŸ dostêpnoœæ urz¹dzenia
            IsDeviceAvailable = await CheckDeviceAvailability(SensorIp);

            return Page();
        }

        private async Task<bool> CheckDeviceAvailability(string ipAddress)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ipAddress, 1000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void EnsureConfigFileExists()
        {
            if (!System.IO.File.Exists(_configFilePath))
            {
                var config = new JObject
                {
                    ["SensorIp"] = DefaultSensorIp
                };
                System.IO.File.WriteAllText(_configFilePath, config.ToString());
            }
        }
    }
}
