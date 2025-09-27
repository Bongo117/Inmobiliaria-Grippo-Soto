using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class TipoInmueble
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del tipo es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
        public string? Descripcion { get; set; }
        
        [Required(ErrorMessage = "Debe especificar el uso permitido")]
        [Display(Name = "Uso Permitido")]
        public string UsoPermitido { get; set; } = string.Empty; // Comercial, Residencial, Mixto
        
        [Display(Name = "Es Comercial")]
        public bool EsComercial { get; set; }
        
        public bool Estado { get; set; } = true;
    }
}
