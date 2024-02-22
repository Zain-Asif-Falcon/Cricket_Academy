using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PlayerMemberShip : GenericModel
    {

        [Display(Name = "Rider Name")]
        public string PlayerName { get; set; }
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }
        public string Address { get; set; }      
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Height { get; set; }      
        [Display(Name = "Parent/Guardian Email")]
        public string ParentOrGuardian_Email { get; set; }
        [Display(Name = "Parent/Guardian Organization")]
        public string ParentOrGuardian_Organization { get; set; }
        public bool HasExperience { get; set; }
        [Display(Name = "Experience Details")]
        public string ExperienceDetails { get; set; }       
        public ICollection<PackageHistory> PackageHistory { get; set; }
    }
}
