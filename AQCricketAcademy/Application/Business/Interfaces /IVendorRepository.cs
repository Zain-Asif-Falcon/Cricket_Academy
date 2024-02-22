using Application.Business.GenericInterfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.Interfaces
{
    public interface IVendorRepository : IGenericInterface<Vendor>
    {
        IEnumerable<Vendor> GetActiveItemVendors();
        bool CheckDuplicateVendor(string VendorName, string VendorAddress, int id);
        string VendorCode();
    }
}
