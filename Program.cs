using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Repository;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

#region Database Config
// Add framework services
services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

//Authentication and authorization
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 4;
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
//.AddDefaultTokenProviders();

services.AddMemoryCache();

services.AddSession();

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

#endregion

#region Services
services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<IEventRepository, EventRepository>();
services.AddScoped<IOrganizationRepository, OrganizationRepository>();
services.AddScoped<ITaskRepository, TaskRepository>();
services.AddScoped<IUserRepository, UserRepository>();
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

//Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LogIn}/{id?}");

//Seed database
DbInitializer.Seed(app);
DbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
