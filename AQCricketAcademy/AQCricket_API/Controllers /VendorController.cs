using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure;
using Application.Business.UnitOfWork;
using Microsoft.AspNetCore.Authorization;

namespace Rider_API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        //private readonly RiderDbContext _context;

        public VendorController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }
       
        // POST: api/ItemVendor
        [HttpPost]
        public async Task<bool> PostItemVendor(Vendor itemVendor, [FromQuery] string UserId)
        {
            bool duplicate = _UnitOfWork.Vendors.CheckDuplicateVendor(itemVendor.VendorName, itemVendor.VendorAddress, itemVendor.id);
            if (duplicate)
            {
                return false;
            }
            else
            {
                if (itemVendor.id == 0)
                {

                    itemVendor.CreatedAt = DateTime.Now;
                    itemVendor.CreatedBy = UserId;
                    _UnitOfWork.Vendors.Add(itemVendor);
                    await _UnitOfWork.SaveDbChanges();
                    return true;

                }
                else
                {
                    itemVendor.UpdatedAt = DateTime.Now;
                    itemVendor.UpdatedBy = UserId;
                    _UnitOfWork.Vendors.Update(itemVendor);
                    await _UnitOfWork.SaveDbChanges();
                    return true;
                }
            }
            //return CreatedAtAction("GetItemVendor", new { id = itemVendor.id }, itemVendor);
        }      

    }
}
