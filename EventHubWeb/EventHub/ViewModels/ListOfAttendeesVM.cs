using EventHub.Models;
using Models;

namespace EventHubWeb.ViewModels
{
    public class ListOfAttendeesVM
    {
        public IEnumerable<EventTicket> tickets { get; set; }
        public Event _event { get; set; }
        public int? SearchedTicketID { get; set; }
    }
}
