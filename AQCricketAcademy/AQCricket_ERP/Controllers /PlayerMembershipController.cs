using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.DTO;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;
using AQCricket_ERP.Common.Utilities;
using System.Diagnostics.Metrics;
using Application.Business.UnitOfWork;
using Newtonsoft.Json.Linq;
using MailKit.Search;
using Org.BouncyCastle.Asn1.X509;
using Rotativa.AspNetCore;
using Domain.Helper;


namespace AQAcademy_ERP.Controllers
{
    public class PlayerMembershipController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IWebHostEnvironment Environment;
        public PlayerMembershipController(IConfiguration configuration, IWebHostEnvironment _env)
        {
            _Configuration = configuration;
            Environment = _env;
        }
        public async Task<IActionResult> Index(int? status = 0)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<PlayerMembershipDTO> AllPlayers = null;
            List<int>? playerIds = null;

            if (status == 1)
            {
                using (var member = new HttpClient())
                {
                    member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    member.DefaultRequestHeaders.Accept.Clear();
                    member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage response = await member.GetAsync("/PlayerMemberShip/GetExpiringPlayers");
                    if (response.IsSuccessStatusCode)
                    {
                        playerIds = await response.Content.ReadAsAsync<List<int>>();
                    }
                }
            }
            if (status == 2)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage response = await client.GetAsync("/PlayerMemberShip/GetExpiredPlayers");
                    if (response.IsSuccessStatusCode)
                    {
                        playerIds = await response.Content.ReadAsAsync<List<int>>();
                    }
                }
            }
            using (var member = new HttpClient())
            {
                member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                member.DefaultRequestHeaders.Accept.Clear();
                member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await member.GetAsync("/PlayerMemberShip/GetAllMembers");
                if (response.IsSuccessStatusCode)
                {
                    AllPlayers = await response.Content.ReadAsAsync<List<PlayerMembershipDTO>>();
                    if (status != 0)
                    {
                        AllPlayers = (from x in AllPlayers
                                      join y in playerIds on x.PlayerId equals y
                                      select x).ToList();
                    }

                    AllPlayers = AllPlayers.OrderByDescending(x => x.CreatedAt).ToList();
                }
            }

            return View(AllPlayers);
        }
        //On save button this method is called wich calls the api to save player, package history , and item sale order and sale goods as well
        //CALLED THROUGH AJAX      
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            var UserId = HttpContext.Session.GetObject<string>("UserId");
            using (var member = new HttpClient())
            {
                member.BaseAddress = new Uri(_Configuration.GetSection("Api_Link").Value);
                member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                //HTTP POST
                var DeleteMember = member.DeleteAsync("PlayerMemberShip?id=" + id);
                DeleteMember.Wait();

                var result = DeleteMember.Result;
                if (!result.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return Ok();
        }    

        [HttpPost]

        //It will save the player using same api as create and then download the receipt for all payments the player made
        public async Task<IActionResult> SaveAndPrint()
        {
            var RequestData = Request.Form;

            PlayerMemberShip NewMember = JsonConvert.DeserializeObject<PlayerMemberShip>(RequestData["ModelData"]);
            if (RequestData.Files?.Count > 0)
            {
                NewMember.File = RequestData.Files[0];
            }
            //NewMember.ItemSaleOders = new List<ItemSaleOders>();
            if (NewMember != null)
            {
                string Token = HttpContext.Session.GetObject<string>("Token");
                if (string.IsNullOrWhiteSpace(Token))
                {
                    TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                    return LocalRedirect("/Identity/Account/Login");
                }
                var UserId = HttpContext.Session.GetObject<string>("UserId");

                if (NewMember.id == 0)
                { //Remove name
                    NewMember.CreatedBy = UserId;
                    NewMember.CreatedAt = Helpers.GetCurrentDateTime();
                }
                else
                { //Remove name
                    NewMember.UpdatedBy = UserId;
                    NewMember.UpdatedAt = Helpers.GetCurrentDateTime();
                }
                if (NewMember.ItemSaleOders?.Count > 0)
                {
                    ItemSaleOders ItemSaleOrder = NewMember.ItemSaleOders[0];
                    ItemSaleOrder.IsPlayerRegistered = true;
                    if (NewMember.id == 0)
                    { //Remove name
                        ItemSaleOrder.CreatedAt = Helpers.GetCurrentDateTime();
                        ItemSaleOrder.CreatedBy = UserId;
                    }
                    else
                    { //Remove name
                        ItemSaleOrder.PlayerId = NewMember.id;
                        ItemSaleOrder.CreatedBy = NewMember.CreatedBy;
                        ItemSaleOrder.CreatedAt = NewMember.CreatedAt;
                        //ItemSaleOrder.UpdatedAt = DateTime.Now;
                        //ItemSaleOrder.UpdatedBy = UserId;
                    }
                    foreach (var item in ItemSaleOrder.ItemSaleGoods)
                    {
                        if (NewMember.id == 0)
                        { //Remove name
                            ItemSaleOrder.SaleOrderDate = Helpers.GetCurrentDateTime();
                            ItemSaleOrder.DueDate = Helpers.GetCurrentDateTime();
                            item.CreatedBy = UserId;
                            item.CreatedAt = Helpers.GetCurrentDateTime();
                        }
                        else
                        {
                            item.CreatedBy = NewMember.CreatedBy;
                            item.CreatedAt = NewMember.CreatedAt;
                            //item.UpdatedBy = UserId;
                            //item.UpdatedAt = DateTime.Now;
                        }
                    }
                }

                if (NewMember.File != null)
                {
                    string path = "/Images/PlayerImages/";
                    string fileName = Path.GetFileName(NewMember.File.FileName);
                    string paths = Path.Combine(Environment.WebRootPath, "Images/PlayerImages", fileName);
                    if (!Directory.Exists(paths))
                    {
                        Directory.CreateDirectory(path);
                    }
                    NewMember.ProfilePicture = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(paths, FileMode.Create))
                    {
                        NewMember.File.CopyTo(stream);
                    }
                }
                NewMember.File = null;


                using (var Member = new HttpClient())
                {
                    //var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    Member.DefaultRequestHeaders.Accept.Clear();
                    Member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage response = await Member.PostAsJsonAsync<PlayerMemberShip>("/PlayerMembership/AddOrEditMember", NewMember);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }
                }
                NewMember.ItemSaleOders = null;
                Packages Package = new Packages();
                using (var GetPackage = new HttpClient())
                {
                    GetPackage.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    GetPackage.DefaultRequestHeaders.Accept.Clear();
                    GetPackage.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    GetPackage.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage Response = await GetPackage.GetAsync("/api/Packages/" + NewMember.PackageId);
                    if (!Response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }
                    Package = await Response.Content.ReadAsAsync<Packages>();
                    NewMember.Package = Package;
                    NewMember.Package.PackageName = Package.PackageName;
                    NewMember.Package.Price = Package.Price;
                    NewMember.CreatedAt = Helpers.GetCurrentDateTime();
                }

                var pdf = new Rotativa.AspNetCore.ViewAsPdf(NewMember)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
                };
                var byteArray = await pdf.BuildFile(ControllerContext);
                return Json(new { file = Convert.ToBase64String(byteArray) });

            }
            else
            {
                return BadRequest();
            }
        }
    }
}
