using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using reffffound.Data;
using reffffound.Models;
using reffffound.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder( args );

// Set Up connection contexts 
//var connectionString = builder.Configuration.GetConnectionString("DataConnection") ?? throw new InvalidOperationException("Default Connection string 'DataConnection' not found.");
//if(string.IsNullOrEmpty(connectionString)) throw new InvalidConfigurationException("Data Connection String is missing");

//var applicationDBConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBConnection' not found.");
//if(string.IsNullOrEmpty(applicationDBConnectionString)) throw new InvalidConfigurationException("ApplicationDBConnection Connection String is missing");

bool isDocker = System.IO.File.Exists( "/proc/1/cgroup" );
string connectionKey = isDocker ? "DockerApplicationLocalDBConnection" : "ApplicationLocalDBConnection";
var applicationLocalDBConnectionString = builder.Configuration.GetConnectionString( connectionKey ) ?? throw new InvalidOperationException( "Connection string 'ApplicationDBConnection' not found." );
if (string.IsNullOrEmpty( applicationLocalDBConnectionString )) throw new InvalidConfigurationException( connectionKey + " Connection String is missing" );


// Add services to the container.
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(applicationDBConnectionString));
builder.Services.AddDbContext<ApplicationDbContext>( options => options.UseSqlite( applicationLocalDBConnectionString ) );
builder.Services.AddDatabaseDeveloperPageExceptionFilter( );

builder.Services.AddDefaultIdentity<ApplicationUser>( options => options.SignIn.RequireConfirmedAccount = false )
	 .AddEntityFrameworkStores<ApplicationDbContext>( ).AddDefaultTokenProviders( );

builder.Services.AddScoped<IUserService, UserService>( );
builder.Services.AddScoped<IBookmarkService, BookmarkContextService>( );
builder.Services.AddSingleton( builder.Configuration );

builder.Services.AddControllersWithViews( );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment( ))
{
	app.UseMigrationsEndPoint( );
}
else
{
	app.UseExceptionHandler( "/Home/Error" );
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts( );
}

app.UseHttpsRedirection( );
app.UseStaticFiles( );

app.UseRouting( );
app.UseAuthorization( );
app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Bookmarks}/{action=Index}/{id?}" );
app.MapRazorPages( );

// 
if (true || isDocker)
{
	using (var scope = app.Services.CreateScope( ))
	{
		Console.WriteLine("Working in " + Environment.CurrentDirectory);
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>( );
		var filelist = new List<string>()
		{
			"app.db", "/app.db", "//app.db/"
		};

		var pathList = new List<string>()
		{
			"", "app", "app/", "app//"
		};

		foreach ( var path in pathList )
		{
			foreach ( var file in filelist )
			{
				var filepath = path + file;
				if(File.Exists(filepath))
				{
					Console.WriteLine("Exists: " +  filepath);
				}
				else
				{
					Console.WriteLine("Not Exists: " + filepath);
				}
			}
		}

		

		if(dbContext.Database.CanConnect())
		{
			Console.WriteLine("SUCCESS: database app.db IS available at " + applicationLocalDBConnectionString);

			if(!dbContext.Bookmarks.Any())
			{
				Console.WriteLine("No bookmarks in _data");
			}
			else
			{
				int count = dbContext.Bookmarks.Count();
				Console.WriteLine(count + " bookmarks in _data");
			}
		}
		else
		{
			Console.WriteLine("ERROR: database app.db IS NOT available at " + applicationLocalDBConnectionString);
		}
	} 
}

app.Run( );
