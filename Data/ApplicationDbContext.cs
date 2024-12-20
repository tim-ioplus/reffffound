using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using reffffound.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace reffffound.Data
{
	public class ApplicationUser : IdentityUser
	{
		public override string UserName { get; set; } = "";
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base( options ) { }
		public DbSet<Bookmark> Bookmarks { get; set; }
		public DbSet<ContentUser> ContentUsers { get; set; }
	}
}
