using SPA_Invoice.Infrastructure.Abstractions;

namespace SPA_Invoice.Infrastructure.Exception
{
    public class NullEmailSetting : IEmailSetting
    {
        public T GetSettingValue<T>(string key)
        {
            return default(T);
        }
    }
}