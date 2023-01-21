using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models;
public class Comment
{
    [Key]
    public int CommentId { get; set; }
    [Required]
    [Column(TypeName = "varchar(250)")]
    public string CommentText { get; set; }
    
    public ApplicationUser User { get; set; }
}
