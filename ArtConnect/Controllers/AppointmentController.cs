//added 11.21.24: created control for frontend  of meeting appointments in database

using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ArtConnect.Models;
using ArtConnect.Services;
using ArtConnect.ViewModels;

namespace ArtConnect.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IOwnerService _ownerService; //also references back to owners to know who the meeting belongs to

        public AppointmentController(IAppointmentService appointmentService, IOwnerService ownerService)
        {
            _appointmentService = appointmentService;
            _ownerService = ownerService;
        }

        public IActionResult Index()
        {
            AppointmentListViewModel viewModel = new AppointmentListViewModel()
            {
                Appointments = _appointmentService.GetAllAppointments(),
            };
            return View(viewModel);
        }

        //11.26.24 - 11.27.24: modified, Render the Add Appointment form
        public IActionResult Add(ObjectId ownerId)
        {
            var selectedOwner = _ownerService.GetOwnerById(ownerId);
            if (selectedOwner == null) return NotFound();

            // 11.27.24: Explicitly map Owner data to AppointmentAddViewModel
            AppointmentAddViewModel appointmentAddViewModel = new AppointmentAddViewModel();

            appointmentAddViewModel.Appointment = new Appointment();
            appointmentAddViewModel.Appointment.OwnerId = selectedOwner.Id;
            appointmentAddViewModel.Appointment.OwnertimeZone = ParseTimeZone(selectedOwner.timeZone);
            appointmentAddViewModel.Appointment.date = DateTime.UtcNow; // 11.27.24: Default date value
            appointmentAddViewModel.Appointment.description = string.Empty; // 11.27.24: Default description
            appointmentAddViewModel.Appointment.format = "Zoom"; // 11.27.24: Default meeting format
            appointmentAddViewModel.Appointment.Link = string.Empty;
            appointmentAddViewModel.Appointment.timeAllotted = int.MaxValue;

            return View(appointmentAddViewModel);
        }
        // Helper method to parse timeZone

        //12.18.24 fixed timezone error - with private helper parsing method
        private DateTimeOffset ParseTimeZone(string? timeZone)
        {
            if (string.IsNullOrEmpty(timeZone)) return DateTimeOffset.UtcNow; // 12.18.24 Default to UTC
            try
            {
                return DateTimeOffset.UtcNow.ToOffset(TimeSpan.Parse(timeZone)); // 12.18.24 Convert string to offset
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid timeZone format: {timeZone}");
                return DateTimeOffset.UtcNow; // Fallback to UTC
            }
        }

        // 11.27.24: Process the Add Appointment form submission
        [HttpPost]
        public IActionResult Add(AppointmentAddViewModel appointmentAddViewModel)
        {
                Appointment newAppointment = new()
                {
                    OwnerId = appointmentAddViewModel.Appointment.OwnerId,
                    date = appointmentAddViewModel.Appointment.date,
                    description = appointmentAddViewModel.Appointment.description,
                    format = appointmentAddViewModel.Appointment.format,
                    Link = appointmentAddViewModel.Appointment.Link
                };
                _appointmentService.AddAppointment(newAppointment);
                return RedirectToAction("Index");
        }
        
        public IActionResult Edit(string Id)
        {
            if (Id == null || string.IsNullOrEmpty(Id)) return NotFound(); //returns 404 screen
            
            var selectedAppointment = _appointmentService.GetAppointmentById(new ObjectId(Id));
            return View(selectedAppointment);
        }

        [HttpPost]
        public IActionResult Edit(Appointment appointment)
        {
            try
            {
                var existingAppointment = _appointmentService.GetAppointmentById(appointment.Id);
                if (existingAppointment != null)
                {
                    _appointmentService.EditAppointment(appointment);
                    return RedirectToAction("Index");
                }
                else ModelState.AddModelError("", $"Appointment with {appointment.Id} ID does not exist.");
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", $"Failed to update appointment details. Error: {ex.Message}");
            }
            return View(appointment);
        }

        public IActionResult Delete(string Id)
        {
            if (Id == null || string.IsNullOrEmpty(Id)) return NotFound();

            var selectedAppointment = _appointmentService.GetAppointmentById(new ObjectId(Id));
            return View(selectedAppointment);
        }

        [HttpPost]
        public IActionResult Delete(Appointment appointment)
        {
            var selectedAppointment = _appointmentService.GetAppointmentById(appointment.Id); //11.27.24: added in, unsure if it will work

            if(appointment.Id == null)
            {
                ViewData["ErrorMessage"] = $"Deleting appointment{appointment.Id} failed. Invalid ID.";
                return View();
            }
            try
            {
                _appointmentService.DeleteAppointment(appointment);
                TempData["AppointmentDeleted"] = "Appointment deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ("", $"Deleting the appointment failed. Error: {ex.Message}");
            }
            return View(selectedAppointment);
        }
    }
}
