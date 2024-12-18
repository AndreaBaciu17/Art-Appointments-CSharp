// file created 11.19.24
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using ArtConnect.Models;

namespace ArtConnect.Services
{
    public class OwnerService : IOwnerService // 11.19.24: implement 'IArtService.cs' interface service file
    {
        private readonly ArtAppointmentsDbContext _dbContext;

        public OwnerService(ArtAppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddOwner(Owner newOwner)
        {
            _dbContext.Owners.Add(newOwner);

            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView); //not for production purposes

            _dbContext.SaveChanges();
        }

        public void DeleteOwner(Owner owner)
        {
            var ownerToDelete = _dbContext.Owners.Where(c => c.Id == owner.Id).FirstOrDefault();

            if (ownerToDelete != null)
            {
                _dbContext.Owners.Remove(ownerToDelete);

                _dbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);

                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The owner name to delete cannot be found.");
            }
        }

        public void EditOwner(Owner updatedOwner)
        {
            var ownerToUpdate = _dbContext.Owners.FirstOrDefault(c => c.Id == updatedOwner.Id);

            if (ownerToUpdate != null)
            {
                ownerToUpdate.ownerName = updatedOwner.ownerName;
                ownerToUpdate.style = updatedOwner.style;
                ownerToUpdate.aboutMe = updatedOwner.aboutMe;
                ownerToUpdate.contactInfo = updatedOwner.contactInfo;
                ownerToUpdate.timeZone = updatedOwner.timeZone;

                _dbContext.Owners.Update(ownerToUpdate);

                _dbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView); //writes changes

                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The owner to update cannot be found.");
            }
        }

        //following classes gets the 'owner' collection's display unit and ID
        public IEnumerable<Owner> GetAllOwners()
        {
            return _dbContext.Owners.OrderByDescending(c => c.Id).Take(5).AsNoTracking().AsEnumerable(); //MongoDB '.id' document object type to order documents
        }                           // to load by Ascending, aka older document in first view, then insert '.OrderBy' 

        public Owner? GetOwnerById(ObjectId id)
        {
            return _dbContext.Owners.FirstOrDefault(c => c.Id == id);
        }
    }
}
