
using Application.Business.GenericInterfaces;
using Application.Business.Interfaces;
using Domain.DTO;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business
{
    class ItemSaleOrderRepository : GenericInterfaceImplementation<ItemSaleOders>, IItemSaleOrderRepository
    {
        private readonly CandidateDataContext _Context;

        public ItemSaleOrderRepository(CandidateDataContext _context) : base(_context)
        {
            _Context = _context;
        }
        public List<ItemSaleOrderDTO> GetActiveSaleOrders()
        {
            return (from s in _Context.ItemSaleOders.Where(x => x.IsActive == true).Include(x => x.ItemSaleGoods).ThenInclude(x => x.Item)
                    join r in _Context.PlayerMemberShip on s.PlayerId equals r.id into sr
                    from so in sr.DefaultIfEmpty()                 
                        //from hs in sh.DefaultIfEmpty()
                    select new ItemSaleOrderDTO
                    {
                        OrderId = s.id,
                        Player = so.PlayerName,                    
                        IsReceipt = s.IsReceipt
                    }).ToList();
            //return _Context.ItemSaleOders.Include("ItemSaleGoods").Where(x => x.IsActive == true).ToList();
        }
        public ItemSaleOders GetSaleOrderDetail(int? id)
        {
            return _Context.ItemSaleOders.Include("ItemSaleGoods").Where(x => x.id == id).FirstOrDefault();
        }
        public async Task<ItemSaleOders> DeleteOrder(int? id)
        {
            return await _Context.ItemSaleOders.FindAsync(id);
        }       
    }
}
