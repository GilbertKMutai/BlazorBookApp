namespace BlazorBookApp.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddHttpClient("BlazorBookApp.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorBookApp.Server"));


        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
        builder.Services.AddScoped<IRecentSearchService, RecentSearchService>();
        builder.Services.AddScoped<IModalService, ModalService>();
        builder.Services.AddScoped<IErrorHandlerService, ErrorHandlerService>();

        await builder.Build().RunAsync();
    }
}
