namespace AppointmentScheduling.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // New property for custom error messages
        public string? ErrorMessage { get; set; }
    }
}
