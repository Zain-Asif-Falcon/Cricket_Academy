using Application.Business.UnitOfWork;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public UserRolesController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }       
        [Route("GetRoleMenuPermissions")]
        [HttpPost]
        public List<RoleMenuPermission> GetRoleMenuPermissions(IEnumerable<string> userRoles)
        {
            var roleMenus = _UnitOfWork.DataAccess.GetRoleMenuPermissions(userRoles);
            return roleMenus;
        }
    }
}
