using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Taller.Frontend.Repositories;
using Taller.Frontend.Components.Pages.Shared;
using EmployeeEntity = Taller.Shared.Entities.Employee;

namespace Taller.Frontend.Components.Pages.Employees
{
    public partial class EmployeesIndex
    {
        private List<EmployeeEntity>? employees;
        private MudTable<EmployeeEntity> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/employees";
        private string infoFormat = "{first_item}-{last_item} => {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService Dialogs { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadTotalRecordsAsync();
        }

        private async Task LoadTotalRecordsAsync()
        {
            loading = true;
            var url = $"{baseUrl}/totalRecords";
            if (!string.IsNullOrWhiteSpace(Filter))
                url += $"?filter={Filter}";

            var response = await Repository.GetAsync<int>(url);
            if (response.Error)
            {
                Snackbar.Add((await response.GetErrorMessageAsync())!, Severity.Error);
                return;
            }

            totalRecords = response.Response;
            loading = false;
        }

        private async Task<TableData<EmployeeEntity>> LoadListAsync(TableState state, CancellationToken _)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}/paginated?page={page}&recordsnumber={pageSize}";

            if (!string.IsNullOrWhiteSpace(Filter))
                url += $"&filter={Filter}";

            var response = await Repository.GetAsync<List<EmployeeEntity>>(url);
            if (response.Error || response.Response is null)
            {
                if (response.Error)
                    Snackbar.Add((await response.GetErrorMessageAsync())!, Severity.Error);

                return new TableData<EmployeeEntity>
                {
                    Items = Array.Empty<EmployeeEntity>(),
                    TotalItems = 0
                };
            }

            return new TableData<EmployeeEntity>
            {
                Items = response.Response,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }

        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            IDialogReference dialog;

            if (isEdit)
                dialog = await Dialogs.ShowAsync<EmployeeEdit>("Editar empleado", new DialogParameters { { "Id", id } }, options);
            else
                dialog = await Dialogs.ShowAsync<EmployeeCreate>("Nuevo empleado", options);

            var result = await dialog.Result;
            if (!result!.Canceled)
            {
                await LoadTotalRecordsAsync();
                await table.ReloadServerData();
            }
        }

        private async Task DeleteAsync(EmployeeEntity employee)
        {
            var dlg = await Dialogs.ShowAsync<ConfirmDialog>(
                "Confirmación",
                new DialogParameters { { "Message", $"¿Borrar a {employee.FirstName} {employee.LastName}?" } },
                new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true });

            var result = await dlg.Result;
            if (result!.Canceled) return;

            var response = await Repository.DeleteAsync($"{baseUrl}/{employee.Id}");
            if (response.Error)
            {
                if (response.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    Nav.NavigateTo("/employee");
                else
                    Snackbar.Add((await response.GetErrorMessageAsync())!, Severity.Error);

                return;
            }

            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
            Snackbar.Add("Registro borrado", Severity.Success);
        }
    }
}