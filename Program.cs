using backend.Configurations;
using backend.Persistance;
using backend.Repositories.Store;
using backend.Repositories.UserInfoRepositories;
using backend.Services.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
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

// Serve static files from wwwroot (default)
app.UseStaticFiles();

// Serve template assets (CSS/JS/img)
var templateAssetsPath = Path.Combine(builder.Environment.ContentRootPath, "template");
if (Directory.Exists(templateAssetsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(templateAssetsPath),
        RequestPath = ""
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

app.UseAuthorization();

// Map MVC controller routes for Razor views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Keep API controllers
app.MapControllers();

app.Run();
