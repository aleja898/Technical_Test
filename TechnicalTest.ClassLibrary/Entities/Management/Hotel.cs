using System.ComponentModel.DataAnnotations;
using TechnicalTest.ClassLibrary.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TechnicalTest.ClassLibrary.Entities.Management
{
    public class Hotel : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Nombre del hotel")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "País")]
        public string Pais { get; set; } = string.Empty;

        [Display(Name = "Latitud")]
        public double Latitud { get; set; }

        [Display(Name = "Longitud")]
        public double Longitud { get; set; }

        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        [Required]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El número de habitaciones debe ser mayor a 0")]
        [Display(Name = "Número de habitaciones")]
        public int NumeroHabitaciones { get; set; }

    }
}