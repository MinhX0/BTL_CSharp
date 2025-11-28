using backend.Configurations;
using backend.Persistance;
using backend.Repositories.Store;
using backend.Repositories.UserInfoRepositories;
using backend.Services.Store;
using backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseMySQL(connectionString);
});
// Register cookie authentication as the default for the web app (so [Authorize] works)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// JWT support still available for APIs
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// Add server-side session to persist small bits of user state on the server
builder.Services.AddSession(options =>
{
    // keep session alive for 7 days for "remember me" behavior
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Add localization services + MVC
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot (default - contains CSS, JS, lib)
app.UseStaticFiles();

// Serve template images (still in template/img for now)
var templateImagesPath = Path.Combine(builder.Environment.ContentRootPath, "template", "img");
if (Directory.Exists(templateImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(templateImagesPath),
        RequestPath = "/img"
    });
}

// Serve product images from external directory
var productImagesPath = builder.Configuration["ImagePaths:ProductImages"];
if (!string.IsNullOrWhiteSpace(productImagesPath) && Directory.Exists(productImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(productImagesPath),
        RequestPath = "/images/products"
    });
}

// Serve category images from external directory
var categoryImagesPath = builder.Configuration["ImagePaths:CategoryImages"];
if (!string.IsNullOrWhiteSpace(categoryImagesPath) && Directory.Exists(categoryImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(categoryImagesPath),
        RequestPath = "/images/categories"
    });
}

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// Configure request localization (default to Vietnamese)
var supportedCultures = new[] { new CultureInfo("vi-VN"), new CultureInfo("en-US") };
var requestLocalizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures.ToList(),
    SupportedUICultures = supportedCultures.ToList()
};
app.UseRequestLocalization(requestLocalizationOptions);

// Configure error pages and status code handling
app.UseStatusCodePagesWithReExecute("/Error/{0}");
// Global exception handler (500)
app.UseExceptionHandler("/Error/500");

// Map MVC controller routes for Razor views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Keep API controllers
app.MapControllers();

app.Run();
