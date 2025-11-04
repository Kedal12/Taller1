using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Cities;

public partial class CityCreate
{
    private City city = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int StateId { get; set; }

    private async Task CreateAsync()
    {
        city.StateId = StateId;
        var responseHttp = await Repository.PostAsync("/api/cities", city);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/states/details/{StateId}");
    }
}