using DemoApplication.Models.Domain;
using DemoApplication.Repositories.Interface;
using DemoApplication.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DemoApplicationContextConnection") ?? throw new InvalidOperationException("Connection string 'DemoApplicationContextConnection' not found.");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(op => op.LoginPath = "/UserLogin/UserLogin");

//Redirect if access denied
builder.Services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Dashboard/AccessDenied");


// Add services to the container.
//Register services for User    
builder.Services.AddScoped<IUserAuthentication, UserAuthenticationService>();
builder.Services.AddScoped<IUser, UserService>();

builder.Services.AddControllersWithViews();

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
app.UseAuthentication(); ;
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserLogin}/{action=UserLogin}/{id?}");

app.Run();
