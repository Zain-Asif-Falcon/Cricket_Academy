using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class ItemSaleOrderDTO
    {
        public int OrderId { get; set; }
        public string SaleOrderDate { get; set; }       
        public bool IsReceipt { get; set; }
        public virtual ICollection<ItemSaleGoods> ItemSaleGoods { get; set; }
    }
}
