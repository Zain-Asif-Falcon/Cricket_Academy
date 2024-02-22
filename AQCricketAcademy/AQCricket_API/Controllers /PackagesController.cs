using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure;
using Application.Business.UnitOfWork;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Domain.Helper;

namespace AQAcademy_API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {

        private readonly IUnitOfWork _UnitOfWork;

        public PackagesController(IUnitOfWork UOW)
        {
            // _context = context;
            _UnitOfWork = UOW;

        }

        // GET: api/Packages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Packages>>> GetPackages()
        {
            return _UnitOfWork.Package.GetActivePackages().ToList();
        }

        // GET: api/Packages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Packages>> GetPackages(int id)
        {
            var package = _UnitOfWork.Package.FindById(id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        }    
    }
}
