using System.Web.Http;
using WebActivatorEx;
using DS.API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DS.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c => c.SingleApiVersion("v01", "ONFINANCE API"))
                .EnableSwaggerUi(c=> {
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }
    }
}
