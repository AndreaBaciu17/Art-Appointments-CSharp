//added 11.21.24: created controller for frontend  of account owners in database

using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ArtConnect.Models;
using ArtConnect.Services;
using ArtConnect.ViewModels;

namespace ArtConnect.Controllers
{
    public class OwnerController : Controller //MVC
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService) //contructor
        {
            _ownerService = ownerService;
        }

        //action inidex that retrieves account owners to be viewable
        public IActionResult Index()
        {
            OwnerListViewModel viewModel = new() //var viewModel = new OwnerListViewModel
            {
                Owners = _ownerService.GetAllOwners(),
            };
            return View(viewModel);
        }

        //git action method to returns 'add owner view'
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(OwnerAddViewModel ownerAddViewModel)
        {
            if (ModelState.IsValid)
            {
                var newOwner = ownerAddViewModel.Owner;
                if (newOwner != null)
                {
                    _ownerService.AddOwner(newOwner);
                    return RedirectToAction("Index"); // Redirect to the Owners list
                }
            }            
            return View(ownerAddViewModel);
        }

        public IActionResult Edit(ObjectId id)
        {
            if (id == null || id == ObjectId.Empty)
            {
                return NotFound(); //returns 404 screen
            }
            var selectedOwner = _ownerService.GetOwnerById(id);
            /*if (selectedOwner == null)
            {
                return NotFound();
            }*/
            return View(selectedOwner);
        }

        //updates system if changes are made
        [HttpPost]
        public IActionResult Edit(Owner owner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ownerService.EditOwner(owner);
                    return RedirectToAction("Index"); // Redirect to the Owners list
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Updating the owner failed: {ex.Message}");
                }
            }
            return View(owner);
        }

        public IActionResult Delete(ObjectId id)
        {
            if (id == ObjectId.Empty) return NotFound();

            var selectedOwner = _ownerService.GetOwnerById(id);
            return View(selectedOwner);
        }
        //git post request to delete owner id
        [HttpPost]
        public IActionResult Delete(Owner owner)
        {
            if (owner.Id == ObjectId.Empty)
            {
                ViewData["Error"] = "Failed to delete owner account, invalid ID";
                return View();
            }
            try
            {
                _ownerService.DeleteOwner(owner);
                TempData["OwnerDeleted"] = "Owner account deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = $"Failed to deleted owner account: {ex.Message}";
                ModelState.AddModelError("", $"Deleting the owner failed: {ex.Message}"); //not sure if i need to remove this
            }
            var selectedOwner = _ownerService.GetOwnerById(owner.Id);
            return View(selectedOwner);
        }
    }
}
