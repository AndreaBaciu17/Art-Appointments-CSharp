using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ArtConnect.Models;

namespace ArtConnect.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ArtAppointmentsDbContext _dbContext;

        public AppointmentService(ArtAppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddAppointment(Appointment newAppointment)
        {
            var owner = _dbContext.Owners.FirstOrDefault(c => c.Id == newAppointment.OwnerId);
            if (owner == null)
            {
                throw new ArgumentException("The owner for this appointment cannot be found.");
            }

            // i don't know how to configure links :(
            // newAppointment.Link = $"https://example.com/appointments/{newAppointment.Id}"; 
            newAppointment.Link = $"{newAppointment.Id}"; // i think this is wrong?
            
            
            _dbContext.Appointments.Add(newAppointment);

            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);

            _dbContext.SaveChanges();
        }

        public void DeleteAppointment(Appointment appointment)
        {
            var appointmentToDelete = _dbContext.Appointments.FirstOrDefault(b => b.Id == appointment.Id);

            if (appointmentToDelete != null)
            {
                _dbContext.Appointments.Remove(appointmentToDelete);

                _dbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);

                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The appointment you are trying to delete cannot be found.");
            }
        }

        public void EditAppointment(Appointment updatedAppointment)
        {
            var appointmentToUpdate = _dbContext.Appointments.FirstOrDefault(b => b.Id == updatedAppointment.Id);

            if (appointmentToUpdate != null)
            {
                appointmentToUpdate.date = updatedAppointment.date;

                appointmentToUpdate.description = updatedAppointment.description;
                appointmentToUpdate.format = updatedAppointment.format;
                appointmentToUpdate.timeAllotted = updatedAppointment.timeAllotted;

                _dbContext.Appointments.Update(appointmentToUpdate);

                _dbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);

                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The appointment to update cannot be found.");
            }
        }

        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _dbContext.Appointments.OrderBy(b => b.date).Take(10).AsNoTracking().AsEnumerable();
        }

        public Appointment? GetAppointmentById(ObjectId id)
        {
            return _dbContext.Appointments.FirstOrDefault(b => b.Id == id);
        }
    }
}
