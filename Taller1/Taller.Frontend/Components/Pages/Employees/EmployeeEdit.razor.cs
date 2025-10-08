using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeEdit
{
    private Employee? employee;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var resp = await Repository.GetAsync<Employee>($"api/employees/{Id}");
        if (resp.Error)
        {
            if (resp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                Nav.NavigateTo("/employee");
            else
                Snackbar.Add((await resp.GetErrorMessageAsync())!, Severity.Error);
            return;
        }
        employee = resp.Response!;
    }

    private async Task EditAsync()
    {
        var resp = await Repository.PutAsync("api/employees", employee);
        if (resp.Error)
        {
            Snackbar.Add((await resp.GetErrorMessageAsync())!, Severity.Error);
            return;
        }
        Return();
        Snackbar.Add("Registro guardado", Severity.Success);
    }

    private void Return() => Nav.NavigateTo("/employee");
}