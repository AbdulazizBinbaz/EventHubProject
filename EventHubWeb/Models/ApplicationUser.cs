using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Description { get; set; }
        [ValidateNever]
        public ICollection<ApplicationUser> Followers { get; set; }
        [ValidateNever]
        public ICollection<ApplicationUser> Following { get; set; }
        [ValidateNever]
        public ICollection<Category> Categories { get; set; }
        [ValidateNever]
        public ICollection<Post> Posts { get; set; }
        [ValidateNever]
        public ICollection<Comment> UserComments { get; set; }
        [ValidateNever]
        public ICollection<Like> Likes { get; set; }
        [ValidateNever]
        public ICollection<EventTicket> EventTickets { get; set; }

    }
}
