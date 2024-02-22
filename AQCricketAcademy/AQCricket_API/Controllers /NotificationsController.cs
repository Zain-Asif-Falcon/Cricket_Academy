using Application.Business.UnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Helper;

namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IConfiguration _config;

        public NotificationsController(IUnitOfWork UOW, IConfiguration config)
        {
            _UnitOfWork = UOW;
            _config = config;
        }

        [Route("GetAllNotifications")]
        [HttpGet]
        public List<Notifications> GetAllNotifications()
        {
            var Notify = _UnitOfWork.Notifications.GetActiveNotifications();
            return Notify;
        }    

        [Route("AddPONotifies")]
        [HttpGet]
        public async Task<IActionResult> AddPONotifies()
        {
            try
            {
                //string link = _config["UI_Link"].ToString();
                var noItems = _UnitOfWork.Notifications.GetPendingNoStock();

                foreach (var Pitem in POs)
                {
                    var flag = true;
                    var notifiedToday = false;
                    foreach (var Nitem in Notify)
                    {
                        if (Nitem.PurchaseOrderId == Pitem.id && Nitem.Status != "Acknowledged")
                        {
                            flag = false;
                            break;
                        }
                        else if (Nitem.PurchaseOrderId == Pitem.id && Nitem.Status == "Acknowledged")
                        {
                            if (Nitem.CreatedAt.Date == Helpers.GetCurrentDateTime().Date)
                                notifiedToday = true;
                        }
                    }
             

                foreach (var Sitem in SOs)
                {
                    var flag = true;
                    var notifiedToday = false;
                    foreach (var Nitem in Notify)
                    {
                        if (Nitem.SaleOrderId == Sitem.id && Nitem.Status != "Acknowledged")
                        {
                            flag = false;
                            break;
                        }
                        if(Nitem.SaleOrderId == Sitem.id && Nitem.Status == "Acknowledged" && Sitem.CustomerReceipts.FirstOrDefault().RemainingBalance != 0)
                        {
                            if (Nitem.CreatedAt.Date == Helpers.GetCurrentDateTime().Date)
                                notifiedToday = true;
                        }
                    }
           
                foreach (var item in SaleItems)
                {
                    var flag = true;
                    var notifiedToday = false;
                    foreach (var Nitem in Notify)
                    {
                        if (Nitem.ItemId == item.id && Nitem.Status != "Acknowledged")
                        {
                            flag = false;
                            break;
                        }
                        else if(Nitem.ItemId == item.id && Nitem.Status == "Acknowledged")
                        {
                            if (Nitem.CreatedAt.Date == Helpers.GetCurrentDateTime().Date)
                                notifiedToday = true;
                        }
                    }
                        if (flag && !notifiedToday)
                        {
                            Notifications notify = new();
                            notify.Date = Helpers.GetCurrentDateTime();
                            notify.CreatedBy = "System";
                            notify.UpdatedBy = "System";
                            notify.CreatedAt = Helpers.GetCurrentDateTime();
                            notify.Url = "/Item/Index?id=" + item.id;
                            notify.Heading = item.ItemName;
                            notify.ItemId = item.id;
                            notify.Status = "Pending";
                            notify.Description = " stock is near to empty. Remaining quantity is " + item.Stock ;
                            _UnitOfWork.Notifications.Add(notify);
                            await _UnitOfWork.SaveDbChanges();
                        }
                    
                }
                foreach (var nitem in noItems)
                {
                    var flag = true;
                    var notifiedToday = false;
                    foreach (var Nitem in Notify)
                    {
                        if (Nitem.ItemId == nitem.id && Nitem.Status != "Acknowledged")
                        {
                            flag = false;
                            break;
                        }
                        else if(Nitem.ItemId == nitem.id && Nitem.Status == "Acknowledged")
                        {
                            if (Nitem.CreatedAt.Date == Helpers.GetCurrentDateTime().Date)
                                notifiedToday = true;
                        }
                    }
                        if (flag && !notifiedToday)
                        {
                            Notifications notify = new();
                            notify.Date = Helpers.GetCurrentDateTime();
                            notify.CreatedBy = "System";
                            notify.UpdatedBy = "System";
                            notify.CreatedAt = Helpers.GetCurrentDateTime();
                            notify.Url = "/Item/Index?id=" + nitem.id;
                            notify.Heading = nitem.ItemName;
                            notify.ItemId = nitem.id;
                            notify.Status = "Pending";
                            notify.Description = " stock is Zero. Buy more" ;
                            _UnitOfWork.Notifications.Add(notify);
                            await _UnitOfWork.SaveDbChanges();
                        }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }      
    }
}
