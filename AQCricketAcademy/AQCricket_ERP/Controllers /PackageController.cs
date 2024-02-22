using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AQCricket_ERP.Common.Utilities;

namespace AQAcademy_ERP.Controllers
{
    public class PackageController : Controller
    {
        private readonly IConfiguration _Configuration;
        private string UserId;
        public PackageController(IConfiguration configuration)
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
            IEnumerable<Packages> AllPackages = null;
            using(var Package = new HttpClient())
            {
                Package.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Package.DefaultRequestHeaders.Accept.Clear();
                Package.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Package.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Package.GetAsync("/api/Packages");
                if (!response.IsSuccessStatusCode)
                {
                    return LocalRedirect("/Identity/Account/Login");
                }
                AllPackages = await response.Content.ReadAsAsync<IEnumerable<Packages>>();
            }
            return View(AllPackages);
        }
     
        public async Task<IActionResult> GetPackageDetail(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            Packages Package = new Packages();
            using (var GetPackage = new HttpClient())
            {
                GetPackage.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                GetPackage.DefaultRequestHeaders.Accept.Clear();
                GetPackage.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                GetPackage.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage Response = await GetPackage.GetAsync("/api/Packages/" + id);
                if (!Response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                Package = await Response.Content.ReadAsAsync<Packages>();
            }
            return Ok(Package);
        }
    }
}
