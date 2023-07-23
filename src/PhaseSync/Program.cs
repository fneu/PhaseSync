using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
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
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddScoped<HiveService>();
            builder.Services.Configure<PhaseSyncOptions>(builder.Configuration.GetSection("PhaseSync"));
            builder.Services.AddMudServices();
            builder.Services.AddLettuceEncrypt();

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

            app.MapGet("/api/sync", async context =>
            {
                using var scope = context.RequestServices.CreateScope();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<PhaseSyncOptions>>().Value;

                var attempted = 0;
                var workoutsSynced = 0;
                var zone_failures = 0;
                var errors = 0;

                foreach(var userId in 
                    new Mapped<string, string>(
                        dir => Path.GetFileName(dir),
                        Directory.GetDirectories(options.HiveDirectory)
                    )
                )
                {
                    try
                    {
                        var hive = new FileHive(options.HiveDirectory, userId);
                        var settings = new SettingsOf(hive);
                        if (new SettingsComplete.Of(settings).Value() && new EnableSync.Of(settings).Value())
                        {
                            attempted++;

                            var taoSession = new TAOSession(new TaoToken.Of(settings).Value());
                            var workoutResult = await taoSession.Send(new GetUpcomingWorkout());
                            JsonNode workout;
                            if (workoutResult.Success()){
                                workout = workoutResult.Content();
                            }
                            else
                            {
                                errors++;
                                continue;
                            }
                            var polarSession =
                                new PolarSession(
                                    new PolarEmail.Of(settings).Value(),
                                    new PolarPassword.Of(settings, options.PasswordEncryptionSecret).Value());

                            foreach (var existingTarget in new PhasedTargetCollection(hive))
                            {
                                await polarSession.Send(new DeleteTarget(hive, existingTarget));
                            }

                            var target = new TAOTarget(hive, workout!.ToString());

                            var sportProfileResult = await polarSession.Send(new GetRunningProfile());
                            if (sportProfileResult.Success())
                            {
                                try
                                {
                                    var zones = new TargetZones(target, settings);
                                    var zonesResult = await polarSession.Send(new PostZones(zones, sportProfileResult.Content().ToString(), settings));
                                    if (zonesResult.Success())
                                    {
                                        settings.Update(
                                            new ZoneLowerBounds(
                                                new Mapped<IZone, double>(
                                                    zone => zone.Min(),
                                                    zones
                                                ).ToArray()
                                            )
                                        );
                                    }
                                    else
                                    {
                                        zone_failures++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    zone_failures++;
                                }
                            }
                            else
                            {
                                zone_failures++;
                            }

                            var result = await polarSession.Send(new PostTarget(target, settings));
                            if (result.Success())
                            {
                                workoutsSynced++;
                            }
                            else
                            {
                                errors++;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        errors++;
                    }
                }
                await context.Response.WriteAsync($"Attempted: {attempted}, Succeeded: {workoutsSynced}, Zone Failures: {zone_failures}, Errors: {errors}");
            });

            app.Run();
        }
    }
}