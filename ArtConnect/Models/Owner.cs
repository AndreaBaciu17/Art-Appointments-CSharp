using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Models
{
    [Collection("owner")] //from MongoDB 'ArtConnectHub' cluster, custom 'ArtConnect_Database' dataset
    public class Owner
    {
        public ObjectId Id { get; set; }

        [Display(Name = "Owner")]
        [Required(ErrorMessage = "Your name is required*")]
        public string? ownerName { get; set; }

        [Display(Name = "Style Type")]
        [Required(ErrorMessage = "What kind of art style are you looking for? This feild is required*")]
        public string? style { get; set; } // e.g., abstract, realism

        [Display(Name = "What brought you to my services?")]
        [Required(ErrorMessage = "Please describe who you are. This feild is required*")]
        public string? aboutMe { get; set; }

        [Display(Name = "Contact")]
        [Required(ErrorMessage = "You must write best way for me to contact you. This feild is required*")]
        public string? contactInfo { get; set; }

        [Display(Name = "Your Timezone")]
        [Required(ErrorMessage = "Please insert your timezone, or where you are located, so we are aligned on timing. This feild is required*")]
        public string? timeZone { get; set; } //12.18.24 fixed error (changed feild to string, also in altas cluster)
        
    }
}
