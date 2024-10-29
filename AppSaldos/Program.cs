using AppSaldos.Components;
using AppSaldos.Data;
using Syncfusion.Blazor;


var builder = WebApplication.CreateBuilder(args);

var licenseKey = builder.Configuration.GetValue<string>("Syncfusion:LicenseKey");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<DataService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
