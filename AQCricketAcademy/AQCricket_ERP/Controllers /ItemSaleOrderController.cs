using Domain.DTO;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AQCricket_ERP.Common.Utilities;
using Newtonsoft.Json;

namespace AQAcademy_ERP.Controllers
{
    public class ItemSaleOrderController : Controller
    {
        private readonly IConfiguration _Configuration;

        public ItemSaleOrderController(IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<ItemSaleOrderDTO> SaleOrderHistory = new List<ItemSaleOrderDTO>();
            using (var items = new HttpClient())
            {
                items.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                items.DefaultRequestHeaders.Accept.Clear();    
                HttpResponseMessage response = await items.GetAsync("/api/ItemSaleOrder/GetSaleOrderHistory");
                if (response.IsSuccessStatusCode)
                {
                    SaleOrderHistory = await response.Content.ReadAsAsync<List<ItemSaleOrderDTO>>();
                }
            }
            return View(SaleOrderHistory.OrderByDescending(o => o.OrderId).ToList());
        }

        public async Task<IActionResult> _SaleGoods()
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            ItemSaleGoods isg = new();
            List<Item> ItemsList;
            using (var items = new HttpClient())
            {
                items.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);          
                items.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await items.GetAsync("/api/Item/GetAllItems");
                if (response.IsSuccessStatusCode)
                {
                    ItemsList = await response.Content.ReadAsAsync<List<Item>>();
                    ItemsList = ItemsList.Where(i => i.Stock > 0).ToList();
                    ViewBag.ItemsList = new SelectList(ItemsList, "id", "ItemName");
                }
                else
                {
                    return LocalRedirect("/Identity/Account/Login");
                }
            }
            return PartialView(isg);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            string UserId = HttpContext.Session.GetObject<string>("UserId");
            using (var item = new HttpClient())
            {
                item.BaseAddress = new Uri(_Configuration.GetSection("Api_Link").Value);             
                HttpResponseMessage DeleteItem = await item.DeleteAsync("/api/ItemSaleOrder/DeleteOrder/?id=" + id + "&UserId=" + UserId);
                if (!DeleteItem.IsSuccessStatusCode)
                {
                    return LocalRedirect("/Identity/Account/Login");
                }
            }
            return Json("OK");
        }
    }
}
