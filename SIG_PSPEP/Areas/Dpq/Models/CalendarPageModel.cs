namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class CalendarPageModel
    {
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        public List<ResumoDiaPercentual> Resumos { get; set; } = new List<ResumoDiaPercentual>();
    }

    public class ResumoDiaPercentual
    {
        public string Data { get; set; } = string.Empty; // formato: "2025-11-02"
        public int Presente { get; set; }
        public int Ausente { get; set; }
        public int Dispensado { get; set; }
        public int Justificado { get; set; }
        // Percentuais calculados
        public double PresentePct { get; set; }
        public double AusentePct { get; set; }
        public double DispensadoPct { get; set; }
        public double JustificadoPct { get; set; }
    }
}
