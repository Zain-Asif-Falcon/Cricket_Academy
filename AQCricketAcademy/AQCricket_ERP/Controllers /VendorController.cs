using AQCricket_ERP.Common.Utilities;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace HorseRider_ERP.Controllers
{
    public class VendorController : Controller
    {
        private readonly IConfiguration _Configuration;
        private string UserId;

        public VendorController(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            IEnumerable<Vendor> ItemVendors;
            using (var ItemVendor = new HttpClient())
            {
                ItemVendor.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                ItemVendor.DefaultRequestHeaders.Clear();
                ItemVendor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ItemVendor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await ItemVendor.GetAsync("/api/Vendor");
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                ItemVendors = JsonConvert.DeserializeObject<IEnumerable<Vendor>>(await response.Content.ReadAsStringAsync());
            }
            return View(ItemVendors);
        } 
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            UserId = HttpContext.Session.GetObject<string>("UserId");
            using (var ItemVendor = new HttpClient())
            {
                ItemVendor.BaseAddress = new Uri(_Configuration.GetSection("Api_Link").Value);
                ItemVendor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await ItemVendor.DeleteAsync("api/Vendor/?Id=" + Id + "&UserId=" + UserId);
                //var result = DeleteMember.Result;
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return Json("OK");
        }
    }
}
