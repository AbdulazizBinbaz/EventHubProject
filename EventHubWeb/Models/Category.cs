using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Column(TypeName = "varchar(50)")]
    public string CategoryName { get; set; }
    [ValidateNever]
    public IEnumerable<Event> Events { get; set; }
    [ValidateNever]
    public ICollection<ApplicationUser> ApplicationUsers { get; set; }
}
