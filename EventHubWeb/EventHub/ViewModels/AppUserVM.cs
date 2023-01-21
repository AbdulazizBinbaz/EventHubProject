using EventHub.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EventHubWeb.ViewModels
{
    public class AppUserVM
    {
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public IEnumerable<Category> categories { get; set; }
        [ValidateNever]
        public ApplicationUser UserOwner { get; set; }

        public Post Post { get; set; }
    }
}
