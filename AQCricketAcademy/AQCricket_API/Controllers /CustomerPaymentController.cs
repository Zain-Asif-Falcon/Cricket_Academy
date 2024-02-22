using Application.Business.UnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Helper;

namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPaymentController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public CustomerPaymentController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }

        [Route("GetPaymentMethods")]
        [HttpGet]
        public List<PaymentMethods> GetPaymentMethods()
        {
            var PayMethods = _UnitOfWork.CustomerPayment.GetPaymentMethods();
            return PayMethods;
        }       
    }
}
