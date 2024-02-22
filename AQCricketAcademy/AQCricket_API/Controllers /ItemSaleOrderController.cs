using Application.Business.UnitOfWork;
using Domain.DTO;
using Domain.Entities;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rider_API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemSaleOrderController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public ItemSaleOrderController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }
        [Route("GetAllSaleOrders")]
        [HttpGet]
        public List<ItemSaleOrderDTO> GetAllSaleOrders()
        {
            var SaleOrderList = _UnitOfWork.ItemSaleOrder.GetActiveSaleOrders();
            return SaleOrderList;
        }
        [Route("CreateUpdateSaleOrder")]
        [HttpPost]
        public async Task<IActionResult> CreateSaleOrderAsync(ItemSaleOders iso)
        {
            try
            {
                if(iso.id == 0)
                {
                    iso.CreatedAt = DateTime.Now;
                    _UnitOfWork.ItemSaleOrder.Add(iso);
                    await _UnitOfWork.SaveDbChanges();
                }
                else
                {
                    iso.UpdatedAt = DateTime.Now;
                    _UnitOfWork.ItemSaleOrder.UpdateChildWithParent(iso, "ItemSaleGoods");
                    //_UnitOfWork.ItemSaleOrder.Update(iso);
                    await _UnitOfWork.SaveDbChanges();
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }       
    }
}
