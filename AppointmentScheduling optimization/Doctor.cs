namespace AppointmentScheduling.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DateTime> AvailableSlots { get; set; } = new List<DateTime>(); // Doctor's available time slots
    }
}
