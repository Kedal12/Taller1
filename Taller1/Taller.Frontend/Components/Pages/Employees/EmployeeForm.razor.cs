using Microsoft.AspNetCore.Components;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeForm
{
    [EditorRequired, Parameter] public Employee Employee { get; set; } = default!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    private DateTime? HireDateNullable { get; set; }
    private bool isActive; // estado local para el select

    protected override void OnParametersSet()
    {
        if (Employee is null)
            Employee = new();

        // Sincroniza el estado local con el modelo cada vez que llega el parámetro
        isActive = Employee.IsActive;
        HireDateNullable = Employee.HireDate;
    }

    private async Task HandleSubmit()
    {
        // Copiamos explícitamente del select al modelo
        Employee.IsActive = isActive;

        if (HireDateNullable.HasValue)
            Employee.HireDate = HireDateNullable.Value;

        Console.WriteLine($"[DEBUG] Submit -> Employee.IsActive={Employee.IsActive}");
        await OnValidSubmit.InvokeAsync();
    }
}