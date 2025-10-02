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
        
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Terminación Anticipada")]
        public DateTime? FechaTerminacionAnticipada { get; set; }
        
        [Display(Name = "Motivo de Terminación")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
        public string? MotivoTerminacion { get; set; }
        
        [Display(Name = "Multa Aplicada")]
        public decimal? MultaAplicada { get; set; }
        
        [Display(Name = "Fecha de Aplicación de Multa")]
        [DataType(DataType.Date)]
        public DateTime? FechaAplicacionMulta { get; set; }
        
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
        
        // Propiedades calculadas para terminación anticipada
        [Display(Name = "Tiene Terminación Anticipada")]
        public bool TieneTerminacionAnticipada => FechaTerminacionAnticipada.HasValue;
        
        [Display(Name = "Porcentaje de Tiempo Transcurrido")]
        public double PorcentajeTiempoTranscurrido
        {
            get
            {
                if (!TieneTerminacionAnticipada) return 0;
                
                var fechaTerminacion = FechaTerminacionAnticipada!.Value;
                var duracionTotal = (FechaFin - FechaInicio).TotalDays;
                var tiempoTranscurrido = (fechaTerminacion - FechaInicio).TotalDays;
                
                return (tiempoTranscurrido / duracionTotal) * 100;
            }
        }
        
        [Display(Name = "Monto de Multa Calculado")]
        public decimal MontoMultaCalculado
        {
            get
            {
                if (!TieneTerminacionAnticipada) return 0;
                
                var porcentaje = PorcentajeTiempoTranscurrido;
                var mesesMulta = porcentaje < 50 ? 2 : 1;
                return MontoMensual * mesesMulta;
            }
        }
        
        [Display(Name = "Estado del Contrato")]
        public string EstadoContrato
        {
            get
            {
                if (TieneTerminacionAnticipada)
                    return "Terminado Anticipadamente";
                if (!EsVigente && DateTime.Today > FechaFin)
                    return "Finalizado";
                if (EsVigente)
                    return "Vigente";
                return "Inactivo";
            }
        }
    }
}