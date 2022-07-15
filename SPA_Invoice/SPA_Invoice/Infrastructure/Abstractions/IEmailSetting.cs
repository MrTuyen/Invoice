namespace SPA_Invoice.Infrastructure.Abstractions
{
    public interface IEmailSetting
    {
        T GetSettingValue<T>(string key);
    }
}