namespace AppointmentScheduling.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ArrivalTime { get; set; } // Patient's arrival time
    }
}
