using BlazorSMHI.FuncApi;
using BlazorSMHI.FuncApi.Helpers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace BlazorSMHI.FuncApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<ISMHIService, SMHIService>(client => client.BaseAddress = new Uri("https://opendata-download-metfcst.smhi.se"));
        }
    }
}
