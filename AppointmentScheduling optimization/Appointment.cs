namespace AppointmentScheduling.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime ScheduledTime { get; set; } // Time of the appointment
    }
}
