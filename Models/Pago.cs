using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Pago
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El número de pago es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de pago debe ser mayor a 0")]
        [Display(Name = "Número de Pago")]
        public int NumeroPago { get; set; }
        
        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; }
        
        [Required(ErrorMessage = "El detalle es obligatorio")]
        [StringLength(200, ErrorMessage = "El detalle no puede exceder los 200 caracteres")]
        [Display(Name = "Detalle")]
        public string Detalle { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El importe es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        [Display(Name = "Contrato")]
        public int ContratoId { get; set; }
        
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true; // true = activo, false = anulado
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Usuario Creador")]
        public string? UsuarioCreador { get; set; }
        
        [Display(Name = "Fecha de Anulación")]
        public DateTime? FechaAnulacion { get; set; }
        
        [Display(Name = "Usuario que Anuló")]
        public string? UsuarioAnulacion { get; set; }
        
        // Propiedades de navegación
        public Contrato? Contrato { get; set; }
        
        // Propiedades calculadas
        [Display(Name = "Estado del Pago")]
        public string EstadoTexto => Estado ? "Activo" : "Anulado";
    }
}
