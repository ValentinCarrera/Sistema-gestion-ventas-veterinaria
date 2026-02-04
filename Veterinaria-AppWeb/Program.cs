using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Veterinaria_AppWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Forzar cultura para binding de números
var defaultCulture = new CultureInfo("es-AR");
defaultCulture.NumberFormat.NumberDecimalSeparator = ".";
defaultCulture.NumberFormat.CurrencyDecimalSeparator = ".";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<VeterinariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VeterinariaConnection")));

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
