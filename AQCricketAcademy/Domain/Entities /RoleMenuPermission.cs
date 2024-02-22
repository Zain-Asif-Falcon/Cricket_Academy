using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table(name: "AspNetRoleMenuPermission")]
    public class RoleMenuPermission
    {
        [Key]
        public int RoleMenuPermissionId { get; set; }
        public string RoleId { get; set; }    
    }
}
