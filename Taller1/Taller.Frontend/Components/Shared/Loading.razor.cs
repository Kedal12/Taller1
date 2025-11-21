using Microsoft.AspNetCore.Components;

namespace Taller.Frontend.Components.Shared;

public partial class Loading
{
    [Parameter] public string? Label { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrEmpty(Label))
        {
            Label = "Por favor espera...";
        }
    }
}