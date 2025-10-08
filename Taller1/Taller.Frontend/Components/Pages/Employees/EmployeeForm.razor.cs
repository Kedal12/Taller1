using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Taller.Shared.Entities;

namespace Taller.Frontend.Components.Pages.Employees;

public partial class EmployeeForm
{
    private EditContext editContext = null!;

    [EditorRequired, Parameter] public Employee Employee { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    /// <summary>
    /// Puente nullable para MudDatePicker (que usa DateTime?) y tu entidad (DateTime no-nullable).
    /// </summary>
    private DateTime? HireDateNullable
    {
        get => Employee.HireDate == default ? (DateTime?)null : Employee.HireDate;
        set
        {
            if (value.HasValue)
                Employee.HireDate = value.Value;
            // Si no viene valor (null) NO pisamos; alternativamente:
            // else Employee.HireDate = DateTime.Today;
        }
    }

    protected override void OnInitialized()
    {
        // Si viene sin fecha, puedes inicializar aquí si lo prefieres:
        if (Employee.HireDate == default)
            Employee.HireDate = DateTime.Today;

        editContext = new EditContext(Employee);
    }
}