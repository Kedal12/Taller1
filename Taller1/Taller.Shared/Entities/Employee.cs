using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Taller.Shared.Entities;

public class Employee
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Apellido")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Activo")]
    public bool IsActive { get; set; }

    [Display(Name = "Fecha de Contratación")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime HireDate { get; set; }

    [Display(Name = "Salario")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "El campo {0} debe ser un número positivo.")]
    [Precision(18, 2)] // 👈 define precisión y escala en la BD
    public decimal Salary { get; set; }
}