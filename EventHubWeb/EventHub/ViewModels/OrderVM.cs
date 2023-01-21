using EventHub.Models;
using Models;

namespace EventHubWeb.ViewModels
{
    public class OrderVM
    {
        public Event Event { get; set; }
        public ApplicationUser applicationUser { get; set; }
        public string PaymentStatus { get; set; }

    }
}
