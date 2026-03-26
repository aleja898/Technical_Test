using System.ComponentModel.DataAnnotations;
using TechnicalTest.ClassLibrary.Interfaces;

using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalTest.ClassLibrary.Entities.Users
{
    public class User : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Correo electrónico")]
        public string Mail { get; set; } = string.Empty;
        
        [StringLength(500)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;
    }
}
