using CoolFan.HelpClasses;
using CoolFan.Interfaces;
using CoolFan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CoolFan.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISensorDataFetcher _sensorDataFetcher;
        private readonly IFanControlService _fanControlService;

        public float Temperature { get; private set; }
        public float Humidity { get; private set; }
        public SensorData SensorData { get; private set; }
        public string ErrorMessage { get; private set; }
        public string CommandMessage { get; private set; }

        public IndexModel(ISensorDataFetcher sensorDataFetcher, IFanControlService fanControlService)
        {
            _sensorDataFetcher = sensorDataFetcher;
            _fanControlService = fanControlService;
        }

        public async Task OnGetAsync()
        {
            await FetchSensorDataAsync();
        }

        public async Task<IActionResult> OnPostFetchDataAsync()
        {
            await FetchSensorDataAsync();
            return Page();
        }

        private async Task<JsonResult> FetchSensorDataAsync()
        {
            try
            {
                SensorData = await _sensorDataFetcher.getSensorDataAsync();
                Temperature = SensorData.Temperature;
                Humidity = SensorData.Humidity;

                return new JsonResult(new
                {
                    temperature = SensorData.Temperature,
                    humidity = SensorData.Humidity
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> OnPostTurnFanOnAsync()
        {
            try
            {
                await _fanControlService.turnON();
                CommandMessage = "Fan turned on.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostTurnFanOffAsync()
        {
            try
            {
                await _fanControlService.turnOFF();
                CommandMessage = "Fan turned off.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetSensorDataAsync()
        {
            try
            {
                SensorData = await _sensorDataFetcher.getSensorDataAsync();
                return new JsonResult(new { temperature = SensorData.Temperature, humidity = SensorData.Humidity });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }
    }
}
