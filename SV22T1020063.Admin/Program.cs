<<<<<<< HEAD
using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1020063.Admin;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "LiteCommerce.Admin";
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/AccessDenied";
                    option.ExpireTimeSpan = TimeSpan.FromDays(7);
                    option.SlidingExpiration = true;
                    option.Cookie.HttpOnly = true;
                    option.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

// Configure Session
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(2);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
=======
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
<<<<<<< HEAD
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//Configure Routing
=======

app.UseRouting();

app.UseAuthorization();

>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

<<<<<<< HEAD
//Configure default format
var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

//Configure Application Context
ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    webHostEnvironment: app.Services.GetRequiredService<IWebHostEnvironment>(),
    configuration: app.Configuration
);

//Get Connection String from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB")
    ?? throw new InvalidOperationException("ConnectionString 'LiteCommerceDB' not found.");

// Initialize Business Layer Configuration
SV22T1020063.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();
=======
app.Run();
>>>>>>> 47b8d31b5cba9f97d73b91bc1c666abcd974b432
