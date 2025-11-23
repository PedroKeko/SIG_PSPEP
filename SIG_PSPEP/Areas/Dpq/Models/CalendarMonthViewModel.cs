namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class CalendarMonthViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<EfectivoDiaStatus> Efectivos { get; set; } = new List<EfectivoDiaStatus>();
    }
}
