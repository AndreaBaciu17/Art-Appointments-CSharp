using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Models
{
    [Collection("appointment")]
    public class Appointment
    {
        public ObjectId Id { get; set; }

        [Required]
        public ObjectId OwnerId { get; set; } // Reference to Artist collection

        [Display(Name = "Purpose of meeting:")]
        [Required(ErrorMessage = "Please describe what you are looking for with this appointment? This feild is required*")]
        public string? description { get; set; }

        [Display(Name = "Meeting Format:")]
        public string? format { get; set; }

        public int? timeAllotted { get; set; }

        [Required(ErrorMessage = "Appointment date and time are required")]
        public DateTime date { get; set; }

        [Display(Name = "Your Timezone")]
        public DateTimeOffset? OwnertimeZone { get; set; }

        public string? Link { get; set; }
    }
}