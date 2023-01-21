using MimeKit.Encodings;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class CategoryEvent
    {
        public IEnumerable<Event> events { get; set; }
        public IEnumerable<Category> categories { get; set; }
    }
}
