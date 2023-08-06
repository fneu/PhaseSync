using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using PhaseSync.Blazor.Areas.Identity;
using PhaseSync.Blazor.Data;
using MudBlazor.Services;
using PhaseSync.Blazor.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xive.Hive;
using Xive;
using Yaapii.Atoms.Collection;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Outgoing.Polar;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Outgoing.TAO;
using System.Text.Json.Nodes;
using PhaseSync.Core.Zones;
using System.Net;
using LettuceEncrypt;

namespace PhaseSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddRazorPages();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddScoped<HiveService>();
            builder.Services.AddHostedService<BackgroundSyncService>();
            builder.Services.Configure<PhaseSyncOptions>(builder.Configuration.GetSection("PhaseSync"));
            builder.Services.AddMudServices();
            builder.Services.AddLettuceEncrypt().PersistDataToDirectory(
                new DirectoryInfo(builder.Configuration.GetSection("PhaseSync").GetValue<string>("HiveDirectory")!).Parent!,
                builder.Configuration.GetSection("PhaseSync").GetValue<string>("PasswordEncryptionSecret")!
            );

            builder.WebHost.UseKestrel(k =>
            {
                IServiceProvider appServices = k.ApplicationServices;
                k.Listen(
                    IPAddress.Any, 443,
                    o => o.UseHttps(h =>
                    {
                        h.UseLettuceEncrypt(appServices);
                    }));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Migrate latest database changes during startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();
                
                // Here is the migration executed
                dbContext.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}