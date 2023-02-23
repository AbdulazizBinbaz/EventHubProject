using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class Post
    {
        public int PostId { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        public string PostText { get; set; }
        [ValidateNever]
        public string? PostImageUrl { get; set; }
        [ValidateNever]
        public DateTime Time { get; set; }
        [ValidateNever]
        [ForeignKey("UserId")]
        public ApplicationUser applicationUser { get; set; }
        [ValidateNever]
        public ICollection<Like> Likes { get; set; }
    }
}
