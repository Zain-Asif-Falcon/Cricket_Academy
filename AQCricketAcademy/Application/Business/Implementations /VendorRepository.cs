using Application.Business.GenericInterfaces;
using Application.Business.Interfaces;
using Domain.Entities;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.Implementations
{
    public class VendorRepository : GenericInterfaceImplementation<Vendor>, IVendorRepository
    {
        private readonly CandidateDataContext _Context;
        public VendorRepository(CandidateDataContext Context) : base(Context)
        {
            _Context = Context;
        }            
        public string VendorCode()
        {           
            int count = 0;

            if (string.IsNullOrWhiteSpace(VC))
            {
                VC = "V" + 1.ToString().PadLeft(4, '0');
            }
            else
            {
                count = Convert.ToInt32(VC.Substring(1)) + 1;
                VC = "V" + count.ToString().PadLeft(4, '0');
            }
            return VC;
        }
    }
}
