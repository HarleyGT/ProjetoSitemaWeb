using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMVC.Data;
using SalesWebMVC.Models;
using SalesWebMVC.Services;

using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SalesWebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SalesWebMVCContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SalesWebMVCContext") ?? throw new InvalidOperationException("Connection string 'SalesWebMVCContext' not found.")));;

            // Add SalesRecordService
            builder.Services.AddScoped<SalesRecordService>();

            // Add DepartmentService
            builder.Services.AddScoped<DepartmentService>();

            // Add SellerService
            builder.Services.AddScoped<SellerService>();

            // Add Seeding Service.
            builder.Services.AddScoped<SeedingService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            /*if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error/");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }*/

            if (app.Environment.IsDevelopment())
            {

                var unUS = new CultureInfo("en-US");
                var localizationOptions = new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(unUS),
                    SupportedCultures = new List<CultureInfo> { unUS },
                    SupportedUICultures = new List<CultureInfo> { unUS }
                };

                app.UseRequestLocalization(localizationOptions);
                app.UseDeveloperExceptionPage();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var seedingService = services.GetRequiredService<SeedingService>();
                    seedingService.Seed();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
            
        }
    }
}
