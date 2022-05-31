using GenesisBlog.Data;
using GenesisBlog.Helper;
using GenesisBlog.Models;
using GenesisBlog.Services;
using GenesisBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Custom Services //
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddTransient<DataService>();
builder.Services.AddScoped<DisplayService>();
builder.Services.AddScoped<IEmailSender, BasicEmailService>();
builder.Services.AddScoped<SearchService>();

// Register an instance of SwaggerGen
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments($"{Directory.GetCurrentDirectory()}/wwwroot/GenesisBlog.xml", true);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Genesis Blog API",
        Version = "v1",
        Description = "Serving up Blog Data for the masses, using .NET 6",
        Contact = new OpenApiContact
        {
            Name = "Charles Hall",
            Email = "productofva@gmail.com",
            Url = new Uri("https://www.LinkedIn.com/in/CharlesUnfiltered")
        }
    });
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
await dataService.SetupDBAsync();

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Public API");
    c.InjectStylesheet("/css/swagger.css");
    c.InjectJavascript("/js/swagger.js");
    c.DocumentTitle = "Genesis Blog Public API";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configured a second, custom route - just for BlogPost/Details
app.MapControllerRoute(
    name: "details",
    pattern: "PostDetails/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details" }
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
