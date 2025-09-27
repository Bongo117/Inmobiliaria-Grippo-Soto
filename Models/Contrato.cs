using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }
        
        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFin { get; set; }
        
        [Required(ErrorMessage = "El monto mensual es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto mensual debe ser mayor a 0")]
        [Display(Name = "Monto Mensual")]
        public decimal MontoMensual { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }
        
        public bool Estado { get; set; } = true;
        
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
        
        // Propiedades calculadas
        [Display(Name = "Duración (días)")]
        public int DuracionDias => (FechaFin - FechaInicio).Days + 1;
        
        [Display(Name = "Duración (meses)")]
        public int DuracionMeses 
        { 
            get 
            {
                var diferencia = FechaFin.Year * 12 + FechaFin.Month - (FechaInicio.Year * 12 + FechaInicio.Month);
                return diferencia;
            } 
        }
        
        [Display(Name = "Contrato Vigente")]
        public bool EsVigente => DateTime.Today >= FechaInicio && DateTime.Today <= FechaFin && Estado;
    }
}