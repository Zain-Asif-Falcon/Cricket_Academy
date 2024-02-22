using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VendorPayment : GenericModel
    {
        [ForeignKey("VendorReceipt")]
        public int VendorReceipt_id { get; set; }
        public virtual VendorReceipt VendorReceipt { get; set; }      
        public DateTime PaymentDate { get; set; }
    }
}
