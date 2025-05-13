using PayPalCheckoutSdk.Core;
using System.Reflection;

namespace Dawam_backend.Extensions;

public static class PayPalClient
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!)
        .AddJsonFile("appsettings.json")
        .Build();

    public static PayPalHttpClient GetClient()
    {
        var environment = new SandboxEnvironment(
            Configuration["PayPal:ClientId"]!,
            Configuration["PayPal:ClientSecret"]!
        );
        return new PayPalHttpClient(environment);
    }
}