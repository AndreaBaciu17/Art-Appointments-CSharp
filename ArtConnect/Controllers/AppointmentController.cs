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
        //1.14.25: fixed OwnerId appointment form error
        public IActionResult Add(ObjectId ownerId)
        {
            var selectedOwner = _ownerService.GetOwnerById(ownerId);

            if (selectedOwner == null) return NotFound();

            var viewModel = new AppointmentAddViewModel
            {
                Appointment = new Appointment
                {
                    OwnerId = selectedOwner.Id, // Ensure OwnerId is set
                    date = DateTime.UtcNow,
                    OwnertimeZone = ParseTimeZone(selectedOwner.timeZone) ?? DateTimeOffset.UtcNow // 1.14.25: fixed appointment form error by defaulting to UTC if null
                }
            };
            return View(viewModel);
        }

        //12.18.24 fixed timezone error - with private helper parsing method
        //1.14.25: set default timezone to UTC if null
        private DateTimeOffset? ParseTimeZone(string? timeZone)
        {
            if (string.IsNullOrEmpty(timeZone)) return null; // 12.18.24 Default to UTC //1.14.25: fixed appointment form error by defaulting to null
            try
            {
                return DateTimeOffset.UtcNow.ToOffset(TimeSpan.Parse(timeZone)); // 12.18.24 Convert string to offset
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid timeZone format: {timeZone}");
                return null; //1.14.25: DateTimeOffset.UtcNow; // Fallback to UTC
            }
        }

        // 11.27.24: Process the Add Appointment form submission
        [HttpPost]
        public IActionResult Add(AppointmentAddViewModel appointmentAddViewModel)
        {
            if (!ModelState.IsValid) return View(appointmentAddViewModel); // 1.17.25: fixed error by returning form with warning if it is not valid
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
            if (selectedAppointment == null) return NotFound(); //returns 404 screen

            //1.17.25: added in to get owner info fro 'edit appointment' view
            var owner = _ownerService.GetOwnerById(selectedAppointment.OwnerId); 
            ViewBag.OwnerName = owner?.ownerName ?? "Unknown";

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
