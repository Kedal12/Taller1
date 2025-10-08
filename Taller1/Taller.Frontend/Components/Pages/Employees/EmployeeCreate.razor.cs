using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeCreate
{
    private Employee employee = new()
    {
        IsActive = true,
        HireDate = DateTime.Today
    };

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private async Task CreateAsync()
    {
        var resp = await Repository.PostAsync("api/employees", employee);
        if (resp.Error)
        {
            Snackbar.Add((await resp.GetErrorMessageAsync())!, Severity.Error);
            return;
        }
        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return() => Nav.NavigateTo("/employee");
}