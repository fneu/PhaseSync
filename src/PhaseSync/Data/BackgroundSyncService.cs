using Microsoft.Extensions.Options;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Outgoing.Polar;
using PhaseSync.Core.Outgoing.TAO;
using PhaseSync.Core.Zones;
using System.Text.Json.Nodes;
using Xive.Hive;
using Yaapii.Atoms.Collection;

namespace PhaseSync.Blazor.Data
{
    public sealed class BackgroundSyncService : IHostedService, IDisposable
    {
        private readonly PhaseSyncOptions options;
        private Timer? _timer;

        public BackgroundSyncService(IOptions<PhaseSyncOptions> options)
        {
            this.options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Sync, null, TimeSpan.Zero, TimeSpan.FromHours(2));
            return Task.CompletedTask;
        }

        private void Sync(object? state)
        {
            if (options is null)
            {
                Console.WriteLine($"BACKGROUNDSYNC: skipped, options is null");
                return;
            }
            var attempted = 0;
            var workoutsSynced = 0;
            var zone_failures = 0;
            var errors = 0;

            foreach(var userId in 
                new Mapped<string, string>(
                    Path.GetFileName,
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
                        var workoutResult = taoSession.Send(new GetUpcomingWorkout()).Result;
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
                            polarSession.Send(new DeleteTarget(hive, existingTarget)).RunSynchronously();
                        }

                        var target = new TAOTarget(hive, workout!.ToString());

                        var sportProfileResult = polarSession.Send(new GetRunningProfile()).Result;
                        if (sportProfileResult.Success())
                        {
                            try
                            {
                                var zones = new TargetZones(target, settings);
                                var zonesResult = polarSession.Send(new PostZones(zones, sportProfileResult.Content().ToString(), settings)).Result;
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
                            catch (Exception)
                            {
                                zone_failures++;
                            }
                        }
                        else
                        {
                            zone_failures++;
                        }

                        var result = polarSession.Send(new PostTarget(target, settings)).Result;
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
            Console.WriteLine($"BACKGROUNDSYNC: Attempted: {attempted}, Succeeded: {workoutsSynced}, Zone Failures: {zone_failures}, Errors: {errors}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
