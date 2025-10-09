using Microsoft.AspNetCore.Components;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeForm
{
    [EditorRequired, Parameter] public Employee Employee { get; set; } = default!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    private DateTime? HireDateNullable { get; set; }
    private bool isActive; // estado local que reflejamos en el modelo

    protected override void OnParametersSet()
    {
        if (Employee is null) Employee = new();

        // Sincroniza estado local con el modelo cada vez que llega el parámetro
        isActive = Employee.IsActive;
        HireDateNullable = Employee.HireDate;
    }

    private void OnActiveChanged(bool value)
    {
        isActive = value;
        Employee.IsActive = value; // 👈 forzamos escribir en el modelo
        Console.WriteLine($"[DEBUG] OnActiveChanged -> {value}");
    }

    private async Task HandleSubmit()
    {
        if (HireDateNullable.HasValue)
            Employee.HireDate = HireDateNullable.Value;

        Console.WriteLine($"[DEBUG] Submit -> Employee.IsActive={Employee.IsActive}");
        await OnValidSubmit.InvokeAsync();
    }
}
