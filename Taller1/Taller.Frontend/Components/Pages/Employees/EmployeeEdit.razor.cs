using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using EmployeeEntity = Taller.Shared.Entities.Employee;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeEdit
{
    private EmployeeEntity? employee;

    [Parameter] public int Id { get; set; }

    [Inject] private IRepository Repository { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // Necesario para poder cerrar el MudDialog desde el hijo
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var resp = await Repository.GetAsync<EmployeeEntity>($"/api/employees/{Id}");
        if (resp.Error || resp.Response is null)
        {
            Snackbar.Add((await resp.GetErrorMessageAsync())!, Severity.Error);
            MudDialog.Cancel();
            return;
        }

        employee = resp.Response;
    }

    private async Task SaveAsync()
    {
        if (employee is null) return;

        var resp = await Repository.PutAsync($"/api/employees/{employee.Id}", employee);
        if (resp.Error)
        {
            Snackbar.Add((await resp.GetErrorMessageAsync())!, Severity.Error);
            return;
        }

        Snackbar.Add("Registro guardado", Severity.Success);
        MudDialog.Close(DialogResult.Ok(true)); // hará que el Index recargue
    }

    // <-- Este método es el que usas en ReturnAction="CloseDialog"
    private void CloseDialog() => MudDialog.Cancel();
    // (Si prefieres async: private Task CloseDialog() { MudDialog.Cancel(); return Task.CompletedTask; })
}
