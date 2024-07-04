using CoolFan.HelpClasses;
using CoolFan.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace wentylator.Pages.FanControl
{
    [Authorize]
    public class FanControlModel : PageModel
    {
        public bool IsFanOn { get; set; } = false;
        public bool IsAutoOn { get; set; } = false;

        private readonly IFanControlService _fanControlService;
        private readonly AutomaticArduinoControl _automaticArduinoControl;

        public FanControlModel(IFanControlService fanControlService)
        {
            _fanControlService = fanControlService;
            _automaticArduinoControl = new AutomaticArduinoControl();
        }

        public string ErrorMessage { get; private set; }
        public string CommandMessage { get; private set; }

        public float tresholdOn { get; set; } = 26;
        public float tresholdOff { get; set; } = 22;

        public IActionResult OnPostTurnFanOn()
        {
            try
            {
                _fanControlService.turnON();
                CommandMessage = "Fan turned on.";
                IsFanOn = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostTurnFanOff()
        {
            try
            {
                _fanControlService.turnOFF();
                CommandMessage = "Fan turned off.";
                IsFanOn = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostSetTresholdOn()
        {
            try
            {
                if (float.TryParse(Request.Form["thresholdOn"], out float threshold))
                {
                    tresholdOn = threshold;
                    _automaticArduinoControl.SetTresholdOn(tresholdOn);
                }
                else
                {
                    ErrorMessage = "Nieprawid³owa wartoœæ dla górnego ograniczenia.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostSetTresholdOff()
        {
            try
            {
                if (float.TryParse(Request.Form["thresholdOff"], out float threshold))
                {
                    tresholdOff = threshold;
                    _automaticArduinoControl.SetTresholdOff(tresholdOff);
                }
                else
                {
                    ErrorMessage = "Nieprawid³owa wartoœæ dla dolnego ograniczenia.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostTurnAutoOn()
        {
            try
            {
                _automaticArduinoControl.setAutoModeOn();
                IsAutoOn = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostTurnAutoOff()
        {
            try
            {
                _automaticArduinoControl.setAutoModeOff();
                IsAutoOn = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }
    }
}
