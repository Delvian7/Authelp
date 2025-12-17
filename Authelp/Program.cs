using Authelp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });









builder.Services.AddAuthorization();

var app = builder.Build();








if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    DbInitializer.Seed(db);
}


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();


app.MapGet("/", () => Results.Redirect("/Index"));

app.Run();

