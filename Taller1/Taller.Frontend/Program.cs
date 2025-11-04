using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Taller.Frontend.AuthenticationProviders; // Asegúrate de que este namespace es correcto
using Taller.Frontend.Components;
using Taller.Frontend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Soporte para MudBlazor
builder.Services.AddMudServices();

// 2. Servicios de .NET 8 para Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 3. HttpClient apuntando a tu API (conservando el puerto 7281)
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7281/") // Puerto de tu Taller.Backend
});

// --- Configuración de Autenticación/Autorización ---
// Esta es la configuración que funciona en tu proyecto 'Orders'

// 4. Habilita el [Authorize] y <AuthorizeView>
builder.Services.AddAuthorizationCore();

// 5. Registra TU proveedor de autenticación personalizado.
//    El componente <CascadingAuthenticationState> en App.razor usará este servicio.
//    Esta línea reemplaza la necesidad de 'AddCascadingAuthenticationState()'.
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderTest>();

// 6. Registra tu repositorio
builder.Services.AddScoped<IRepository, Repository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// Se eliminan app.UseAuthentication() y app.UseAuthorization()
// para coincidir con la lógica de tu proyecto 'Orders',
// que delega todo al AuthenticationStateProvider.

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();