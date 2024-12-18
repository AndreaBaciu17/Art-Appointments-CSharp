// 11.19.24: Interface Service file, contract for CRUD classes
using MongoDB.Bson;
using ArtConnect.Models;

namespace ArtConnect.Services
{
    public interface IOwnerService
    {
        //CRUD Operations
        // 11.19.24: create classes for 'GetAllOwners', 'GetOwnerById', 'AddOwner', 'EditOwner', and 'DeleteOwner'
        // 11.19.24: these classes are stored in the 'ArtService.cs' service file
        IEnumerable<Owner> GetAllOwners();
        Owner? GetOwnerById(ObjectId id);

        void AddOwner(Owner newOwner);

        void EditOwner(Owner updatedOwner);

        void DeleteOwner(Owner ownerToDelete);
    }
}
