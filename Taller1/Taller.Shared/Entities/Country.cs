using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taller.Shared.Interfaces;

namespace Taller.Shared.Entities;

public class Country : IEntityWithName
{
    public int Id { get; set; }

    [Display(Name = "País")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;

    public ICollection<State>? States { get; set; }

    public int StatesNumber => States == null ? 0 : States.Count;
}
