namespace FPTJob_1670.Services
{
    public interface ISMSSenderService
    {
        Task SendSMSAsync(string toPhone, string message);
    }
}
