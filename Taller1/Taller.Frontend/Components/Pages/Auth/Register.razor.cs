using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using Taller.Frontend.Services;
using Taller.Shared.DTOs;
using Taller.Shared.Entities;
using Taller.Shared.Enums;

namespace Taller.Frontend.Components.Pages.Auth;

public partial class Register
{
    private UserDTO userDTO = new();
    private List<Country>? countries;
    private List<State>? states;
    private List<City>? cities;
    private bool loading;
    private string? imageUrl;
    private string? titleLabel;

    private Country selectedCountry = new();
    private State selectedState = new();
    private City selectedCity = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public bool IsAdmin { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        titleLabel = IsAdmin ? "Registro de Administrador" : "Registro de Usuario";
    }

    private void ImageSelected(string imageBase64)
    {
        userDTO.Photo = imageBase64;
        imageUrl = null;
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        countries = responseHttp.Response;
    }

    private async Task LoadStatesAsyn(int countryId)
    {
        var responseHttp = await Repository.GetAsync<List<State>>($"/api/states/combo/{countryId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        states = responseHttp.Response;
    }

    private async Task LoadCitiesAsyn(int stateId)
    {
        var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        cities = responseHttp.Response;
    }

    private async Task CountryChangedAsync(Country country)
    {
        selectedCountry = country;
        selectedState = new State();
        selectedCity = new City();
        states = null;
        cities = null;
        await LoadStatesAsyn(country.Id);
    }

    private async Task StateChangedAsync(State state)
    {
        selectedState = state;
        selectedCity = new City();
        cities = null;
        await LoadCitiesAsyn(state.Id);
    }

    private void CityChanged(City city)
    {
        selectedCity = city;
        userDTO.CityId = city.Id;
    }

    private async Task<IEnumerable<Country>> SearchCountries(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return countries ?? new List<Country>();
        }

        return countries!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<State>> SearchStates(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return states ?? new List<State>();
        }

        return states!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<City>> SearchCity(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return cities ?? new List<City>();
        }

        return cities!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }

    private void InvalidForm()
    {
        Snackbar.Add("Por favor llena todos los campos obligatorios.", Severity.Warning);
    }

    // --- ESTA ES LA PARTE CRITICA CORREGIDA ---
    private async Task CreateUserAsync()
    {
        // Validaciones básicas
        if (string.IsNullOrEmpty(userDTO.Email) || string.IsNullOrEmpty(userDTO.PhoneNumber) || userDTO.CityId == 0)
        {
            Snackbar.Add("Verifica que el Email, Teléfono y Ciudad estén seleccionados.", Severity.Warning);
            return;
        }

        userDTO.UserType = IsAdmin ? UserType.Admin : UserType.User;
        userDTO.UserName = userDTO.Email;

        loading = true; // Activar spinner

        try
        {
            var responseHttp = await Repository.PostAsync<UserDTO, TokenDTO>("/api/accounts/CreateUser", userDTO);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return; // Se va al finally
            }

            // Verificación de seguridad del Token
            if (responseHttp.Response is null || string.IsNullOrEmpty(responseHttp.Response.Token))
            {
                Snackbar.Add("Registro exitoso, pero no se recibió el token de acceso.", Severity.Error);
                return; // Se va al finally
            }

            // Login Exitoso
            await LoginService.LoginAsync(responseHttp.Response.Token);

            // Usamos forceLoad: true para limpiar caché de auth y evitar errores en la siguiente página
            NavigationManager.NavigateTo("/", forceLoad: true);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Ocurrió un error inesperado: {ex.Message}", Severity.Error);
        }
        finally
        {
            // ESTO ASEGURA QUE EL SPINNER SIEMPRE SE APAGUE
            loading = false;
        }
    }
}