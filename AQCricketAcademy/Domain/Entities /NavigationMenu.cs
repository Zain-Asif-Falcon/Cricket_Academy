using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	[Table(name: "AspNetNavigationMenu")]
	public class NavigationMenu
    {
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string ActionName { get; set; }

		public bool IsExternal { get; set; }		

		public bool Visible { get; set; }
	}
}
