namespace Inmobiliaria_.Net_Core.Models
{
    public class DashboardViewModel
    {
        public int InmueblesDisponibles { get; set; }
        public int InmueblesOcupados { get; set; }
        public int TotalInmuebles { get; set; }
        public int ContratosVigentes { get; set; }
        public int InquilinosActivos { get; set; }
        public int PropietariosActivos { get; set; }
        public List<Contrato> ContratosPorVencer { get; set; } = new List<Contrato>();
    }
}