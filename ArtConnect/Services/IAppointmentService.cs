// 11.19.24: Interface Service file, contract for CRUD classes
using MongoDB.Bson;
using ArtConnect.Models;

namespace ArtConnect.Services
{
    public interface IAppointmentService
    {
        //CRUD Operations
        // 11.19.24: create classes for 'GetAllAppointments', 'GetAppointmentById', 'AddAppointment', 'EditAppointment', and 'DeleteAppointment'
        // 11.19.24: these classes are stored in the 'ArtService.cs' service file
        IEnumerable<Appointment> GetAllAppointments();
        Appointment? GetAppointmentById(ObjectId id);

        void AddAppointment(Appointment  newAppointment);

        void EditAppointment(Appointment  updatedAppointment);

        void DeleteAppointment(Appointment  appointmentToDelete);
    }
}
