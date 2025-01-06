using AppointmentScheduling.Models;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingSystem.Controllers
{
    namespace SchedulingSystem.Controllers
    {
        public class ScheduleController : Controller
        {
            private static List<Patient> patients = new List<Patient>();
            private static List<Doctor> doctors = new List<Doctor>();
            private static List<Appointment> appointments = new List<Appointment>();

            public ScheduleController()
            {
                // Initialize doctors and their time slots (mock data)
                if (!doctors.Any())
                {
                    // Define available hours from 8 AM to 5 PM (inclusive)
                    var availableHours = new List<int> { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

                    // Get today's date and the last day of the current month
                    DateTime today = DateTime.Today;
                    DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)); // Last day of the current month

                    // Add Dr. Cedric
                    doctors.Add(new Doctor
                    {
                        Id = 1,
                        Name = "Dr. Cedric", // Name set to Dr. Cedric
                        AvailableSlots = new List<DateTime>()
                    });

                    // Add Dr. Merwin
                    doctors.Add(new Doctor
                    {
                        Id = 2,
                        Name = "Dr. Merwin", // Name set to Dr. Merwin
                        AvailableSlots = new List<DateTime>()
                    });

                    // Loop through the days from today to the end of the current month
                    for (DateTime currentDay = today; currentDay <= endOfMonth; currentDay = currentDay.AddDays(1))
                    {
                        // Add available slots for each doctor for the current day
                        foreach (var doctor in doctors)
                        {
                            doctor.AvailableSlots.AddRange(availableHours.Select(hour => currentDay.AddHours(hour)));
                        }
                    }
                }
            }




            [HttpGet]
            public IActionResult Index()
            {
                return View(appointments); // Show scheduled appointments
            }

            [HttpPost]
            public IActionResult Schedule(Patient newPatient)
            {
                // Get today's date (without time) and the last day of the current month
                DateTime today = DateTime.Now.Date;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                // Debug: Print the current values for verification
                Console.WriteLine($"Today: {today}");
                Console.WriteLine($"End of Month: {endOfMonth}");
                Console.WriteLine($"Patient Arrival Date: {newPatient.ArrivalTime.Date}");

                // Validate that the arrival time is within the allowed range
                if (newPatient.ArrivalTime.Date < today || newPatient.ArrivalTime.Date > endOfMonth)
                {
                    var errorViewModel = new ErrorViewModel
                    {
                        ErrorMessage = $"Appointments can only be scheduled from {today:MMMM dd, yyyy} to {endOfMonth:MMMM dd, yyyy}."
                    };
                    return View("Error", errorViewModel);
                }

                // Add the patient to the list
                patients.Add(newPatient);

                // Optimize scheduling
                var optimalAppointment = SchedulePatient(newPatient);

                if (optimalAppointment != null)
                {
                    appointments.Add(optimalAppointment);
                    return RedirectToAction("Index");
                }

                // If no slots are available, show an error
                var noSlotsErrorViewModel = new ErrorViewModel
                {
                    ErrorMessage = "No available slots for this patient."
                };
                return View("Error", noSlotsErrorViewModel);
            }



            private Appointment SchedulePatient(Patient patient)
            {
                // Debug: Print the patient's arrival time
                Console.WriteLine($"Patient Arrival Time: {patient.ArrivalTime}");

                // Get available slots for all doctors on the same day as the patient's arrival
                var availableSlots = doctors
                    .SelectMany(d => d.AvailableSlots.Select(slot => new { Doctor = d, Slot = slot }))
                    .Where(s => s.Slot.Date == patient.ArrivalTime.Date) // Match only slots on the same day
                    .OrderBy(s => s.Slot)
                    .ToList();

                // Debug: Print all available slots
                Console.WriteLine("Available Slots:");
                foreach (var slot in availableSlots)
                {
                    Console.WriteLine($"Doctor: {slot.Doctor.Name}, Slot: {slot.Slot}");
                }

                // Find the closest available slot that matches or is after the patient's arrival time
                var bestSlot = availableSlots.FirstOrDefault(s => s.Slot >= patient.ArrivalTime);

                if (bestSlot != null)
                {
                    // Remove the booked slot from the doctor's availability
                    bestSlot.Doctor.AvailableSlots.Remove(bestSlot.Slot);

                    // Debug: Print the selected slot
                    Console.WriteLine($"Scheduled Appointment: Doctor {bestSlot.Doctor.Name}, Slot {bestSlot.Slot}");

                    return new Appointment
                    {
                        Id = appointments.Count + 1,
                        Patient = patient,
                        Doctor = bestSlot.Doctor,
                        ScheduledTime = bestSlot.Slot
                    };
                }

                // Debug: No slots found
                Console.WriteLine("No available slots found for the patient.");
                return null;
            }


        }
    }
}
