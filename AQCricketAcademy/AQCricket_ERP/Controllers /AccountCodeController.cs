
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using AQCricket_ERP.Common.Utilities;

namespace AQAcademy_ERP.Controllers
{
    public class AccountCodeController : Controller
    {
        private readonly IConfiguration _Configuration;

        public AccountCodeController(IConfiguration configuration)
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
            List<AccountCodes> AccountCodesList = new List<AccountCodes>();
            using (var items = new HttpClient())
            {
                items.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                items.DefaultRequestHeaders.Accept.Clear();
                items.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                items.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await items.GetAsync("/api/AccountCode/GetAllAccountCodes");
                if (response.IsSuccessStatusCode)
                {
                    AccountCodesList = await response.Content.ReadAsAsync<List<AccountCodes>>();
                }
            }
            return View(AccountCodesList.OrderByDescending(i => i.id).ToList());
        }
      
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            var UserId = HttpContext.Session.GetObject<string>("UserId");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            using (var item = new HttpClient())
            {
                item.BaseAddress = new Uri(_Configuration.GetSection("Api_Link").Value);
                item.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage DeleteItem = await item.DeleteAsync("/api/AccountCode/DeleteAccountCode/?id=" + id + "&UserId=" + UserId);
                if (!DeleteItem.IsSuccessStatusCode)
                {
                    string Error = await DeleteItem.Content.ReadAsStringAsync();
                    bool IsErrorMessagePresent = Error.IndexOf("Error") > 0 ? true : false;
                    if (IsErrorMessagePresent)
                    {
                        int startIndex = Error.IndexOf(':') + 1;
                        int endIndex = Error.IndexOf("at", startIndex);
                        string ErrorMessage = Error.Substring(startIndex, endIndex - startIndex).Trim();
                        return BadRequest(ErrorMessage);
                    }
                    else
                    {
                        return LocalRedirect("/Identity/Account/Login");
                    }
                }
            }
            return Json("OK");
        }
    }
}
