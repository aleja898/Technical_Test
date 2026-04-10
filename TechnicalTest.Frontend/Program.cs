using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TechnicalTest.Frontend;
using TechnicalTest.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7091")  
    };
    httpClient.DefaultRequestHeaders.Add("User-Agent", "Blazor Frontend");
    return httpClient;
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<CountryService>();

await builder.Build().RunAsync();