using System.ComponentModel.DataAnnotations;


namespace TechnicalTest.ClassLibrary.Interfaces
{
    public class IEntityDelete : IEntity
    {
        [Display(Name = "Eliminado")]
        public DateTime? Deleted { get; set; } = null;
    }
}
