using EventHub.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Event
    {
        [Key]
        [Display(Name ="Event id")]
        public int EventId { get; set; }
        [Column(TypeName = "varchar(50)")]
        [Required]
        [Display(Name = "Event name")]
        public string EventName { get; set; }
        [Display(Name = "Event price")]
        public int? EventPrice { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        [Display(Name = "Event location")]
        public string EventLocation { get; set; }
        [Required]
        [Display(Name = "Event date")]
        public DateTime EventDate { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        [Display(Name = "Event description")]
        public string EventDescription { get; set; }
        [Required]
        [ValidateNever]
        [Display(Name = "Event picture")]
        public string EventPictureUrl { get; set; }
        [Column(TypeName = "varchar(50)")]
        [ValidateNever]
        [Display(Name = "Event status")]
        public string EventStatus { get; set; }
        [ValidateNever]
        public string EventManagerId { get; set; }
        [ValidateNever]
        [ForeignKey("EventManagerId")]
        [Display(Name = "Event manager")]
        public ApplicationUser applicationUser { get; set; }
        [ValidateNever]
        [Display(Name = "Event categories")]
        public ICollection<Category> categories { get; set; }
        [ValidateNever]
        [Display(Name = "Event comments")]
        public ICollection<Comment> EventComments { get; set; }
        [ValidateNever]
        public ICollection<EventTicket> EventTickets { get; set; }
    }
}