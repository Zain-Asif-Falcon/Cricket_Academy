
using Application.Handlers;
using AQCricket_ERP.Common.Utilities;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AQAcademy_ERP.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly SignInManager<ApplicationUsers> _signInManager;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _Configuration;
        //private readonly IEmailSender _emailSender;

        public UserRolesController(
            UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager,
            ILogger<UserController> logger, RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _Configuration = config;
        }

        // [Authorize]
        public async Task<IActionResult> Index()
        {
            var RolesDTO = new List<UserRolesDTO>();
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var item in roles)
                {
                    if (item.Name != "Super Admin")
                    {
                        RolesDTO.Add(new UserRolesDTO()
                        {
                            Id = item.Id,
                            RoleName = item.Name,
                        });
                    }
                }
                return View(RolesDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        public IActionResult _Create()
        {
            UserRolesDTO userRoles = new();
            return PartialView(userRoles);
        }
      
        [HttpPost]
        public async Task<IActionResult> SaveTreeItems(string selectedItems)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<TreeViewNode> items = JsonConvert.DeserializeObject<List<TreeViewNode>>(selectedItems);

            var childs = items.Where(a => a.parent != "#"); 
            var parents = items.Where(a => a.parent == "#").Select(x => Convert.ToInt32(x.id));
            string id = HttpContext.Session.GetObject<string>("RoleId");

            var permissions = parentsId.Concat(childsId).Concat(parents).Distinct();        
            using (var per = new HttpClient())
            {
                per.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                per.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage PostPermissions = await per.PostAsJsonAsync<IEnumerable<int>>("/api/UserRoles/SetPermissionsByRoleId?id=" + id, permissions);
                if (!PostPermissions.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return RedirectToAction(nameof(Index));
            //}
            //return RedirectToAction(nameof(Index));
        }
    }
}
