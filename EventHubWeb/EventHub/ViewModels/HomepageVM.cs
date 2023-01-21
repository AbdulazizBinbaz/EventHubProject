using EventHub.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System.ComponentModel.DataAnnotations;

namespace EventHubWeb.ViewModels
{
    public class HomepageVM
    {
        [ValidateNever]
        public Event Event { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        [Display(Name ="First Category")]
        public int FirstSelectedCategoryId { get; set; }
        [Display(Name = "Second Category")]
        public int SecondSelectedCategoryId { get; set; }
        [ValidateNever]
        [Display(Name = "Search")]
        public string? SearchText { get; set; }
        [ValidateNever]
        public IEnumerable<Event>? EventList { get; set; }
        [ValidateNever]
        public IEnumerable<Post>? PostList { get; set; }
        [ValidateNever]
        public Comment? Comment { get; set; }
        public ApplicationUser? user { get; set; }
    }
}
