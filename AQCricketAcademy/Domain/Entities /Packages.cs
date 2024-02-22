using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Packages : GenericModel
    {
        [Display(Name = "Package Name")]
        public string PackageName { get; set; }
        [Display(Name = "No Of Classes")]
        public int NumberOfClasses { get; set; }       
        public int TotalPrice { get; set; }
    }
}
