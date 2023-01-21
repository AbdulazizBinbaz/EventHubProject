using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class Like
    {

            public int PostId { get; set; }
            public Post Post { get; set; }
            public string UserId { get; set; }
            public ApplicationUser User { get; set; }
        
    }
}
