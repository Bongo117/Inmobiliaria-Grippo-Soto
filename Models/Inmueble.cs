using System.ComponentModel.DataAnnotations;
using Inmobiliaria_Grippo_Soto.Models;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Inmueble : AuditableEntity
    {
        
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres")]
        public string Tipo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria")]
        [Range(1, 20, ErrorMessage = "La cantidad de ambientes debe estar entre 1 y 20")]
        public int Ambientes { get; set; }
        
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio Mensual")]
        public decimal Precio { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        [Display(Name = "Propietario")]
        public int PropietarioId { get; set; }
        
        
        // Propiedad de navegación
        public Propietario? Propietario { get; set; }
    }
}