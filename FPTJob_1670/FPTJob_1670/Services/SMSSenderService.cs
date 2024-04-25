using FPTJob_1670.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FPTJob_1670.Services
{
    public class SMSSenderService : ISMSSenderService
    {
        private readonly TwilioSettings _twilioSetting;

        public SMSSenderService( IOptions< TwilioSettings> twilioSettings )
        {
            _twilioSetting = twilioSettings.Value;
        }
        public async Task SendSMSAsync(string toPhone, string message)
        {
            TwilioClient.Init(_twilioSetting.AccountSID, _twilioSetting.AuthToken);
            await MessageResource.CreateAsync(
                to: toPhone,
                from: _twilioSetting.FromPhone,
                body: message


                );
        }
    }
}
