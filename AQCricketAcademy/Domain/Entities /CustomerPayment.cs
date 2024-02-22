using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustomerPayment:GenericModel
    {
        [ForeignKey("CustomerReceipt")]
        public int CustomerReceipt_id { get; set; }
        public virtual CustomerReceipt CustomerReceipt { get; set; }       
        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
