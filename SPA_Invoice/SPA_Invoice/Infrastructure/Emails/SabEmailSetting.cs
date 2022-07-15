using SPA_Invoice.Infrastructure.Abstractions;
using Microsoft.Extensions.Configuration;
using System;

namespace SPA_Invoice.Infrastructure.Emails
{
    public class SabEmailSetting : IEmailSetting
    {
        private readonly IConfiguration _config;

        public SabEmailSetting(IConfiguration config)
        {
            _config = config;
        }

        public T GetSettingValue<T>(string key)
        {
            if (_config == null)
            {
                throw new NullReferenceException();
            }

            var configValue = _config[key];
            if (string.IsNullOrEmpty(configValue))
            {
                throw new RankException("Config error");
            }

            return (T)Convert.ChangeType(configValue, typeof(T));
        }
    }

}