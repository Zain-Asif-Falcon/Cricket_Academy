
using AQCricket_ERP.Common.Utilities;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AQAcademy_ERP.Controllers
{
    public class CustomerPaymentController : Controller
    {
        private readonly IConfiguration _Configuration;

        public CustomerPaymentController(IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> _Create(int id)
        {
            var Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<PaymentMethods> PaymentMethods = new();
            using (var method = new HttpClient())
            {
                method.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                method.DefaultRequestHeaders.Accept.Clear();              
                HttpResponseMessage response = await method.GetAsync("/api/VendorPayment/GetPaymentMethods");
                if (response.IsSuccessStatusCode)
                {
                    PaymentMethods = await response.Content.ReadAsAsync<List<PaymentMethods>>();
                    ViewBag.PaymentMethods = new SelectList(PaymentMethods, "id", "PM_Name");
                }
            }
            CustomerPayment cp = new();
            cp.CustomerReceipt_id = id;        
            return PartialView(cp);


        }
        
        }
    }
}
