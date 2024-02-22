using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Business.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.DTO;
using Domain.Helper;

namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCodeController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public AccountCodeController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }
        [Route("GetAccountHeads")]
        [HttpGet]
        public List<AccountHead> GetAccountHeads()
        {
            return _UnitOfWork.AccountCode.GetAccountHeads();
        }     

        [Route("GetAccountCodeById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAccountCodeById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _UnitOfWork.AccountCode.GetAccountCodeInfo(id);
            return Ok(item);
        }
        [Route("GetCodeByAccountHeadId/{id}")]
        [HttpGet]
        public IActionResult GetCodeByAccountHeadId(int id)
        {
            try
            {
                string Code = _UnitOfWork.AccountCode.GetCodeByAccountHeadId(id);
                return Ok(Code);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        }      
    }
}
