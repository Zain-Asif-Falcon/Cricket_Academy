using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Business.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Helper;

namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public ItemController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }   
       
        [Route("AddOrEditItem")]
        [HttpPost]
        public async Task<bool> PostItem(Item item)
        {

            //Item item = null;
            if (_UnitOfWork.Item.CheckDuplicateItem(item.ItemName, item.id))
            {
                throw new Exception("Another Item already exists with this Name");
            }
            else if (_UnitOfWork.Item.CheckDuplicateCode(item.ItemCode, item.id))
            {
                throw new Exception("Another Item already exists with this Code");
            }
            else
            {
                if (item.id == 0)
                {

                    item.CreatedAt = Helpers.GetCurrentDateTime();
                    _UnitOfWork.Item.Add(item);
                    await _UnitOfWork.SaveDbChanges();
                    return true;

                }
                else
                {
                    item.UpdatedAt = Helpers.GetCurrentDateTime();
                    _UnitOfWork.Item.Update(item);
                    await _UnitOfWork.SaveDbChanges();
                    return true;
                }
            }

        }      
    }
}
