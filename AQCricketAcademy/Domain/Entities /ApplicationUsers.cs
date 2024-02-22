using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; 
namespace Domain.Entities
{
    public class ApplicationUsers : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        //public IFormFile File { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
