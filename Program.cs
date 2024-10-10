using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using reffffound.Data;
using reffffound.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Set Up connection contexts 
var connectionString = builder.Configuration.GetConnectionString("DataConnection") ?? throw new InvalidOperationException("Default Connection string 'DataConnection' not found.");
if(string.IsNullOrEmpty(connectionString)) throw new InvalidConfigurationException("Data Connection String is missing");

var applicationDBConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBConnection' not found.");
if(string.IsNullOrEmpty(applicationDBConnectionString)) throw new InvalidConfigurationException("ApplicationDBConnection Connection String is missing");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(applicationDBConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bookmarks}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
