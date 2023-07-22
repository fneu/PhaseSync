using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class TAOTarget : EntityEnvelope<IHoneyComb>
    {
        public TAOTarget(IHive userHive, string taoWorkout) : this(
            userHive,
            JsonNode.Parse(taoWorkout)!
        )
        { }

        public TAOTarget(IHive userHive, JsonNode taoWorkout) : base(() =>
        {
            var target = new PhasedTargetOf(userHive, ((string)taoWorkout["id"]!).Replace("/", "_"));
            var settings = new SettingsOf(userHive);

            var steps = taoWorkout["workoutSteps"]!;
            target.Update(
                new Title((string)taoWorkout["activitySubType"]!),
                new Time((string)taoWorkout["start"]!, "Europe/Berlin"), // TODO: Get timezone from user settings
                new Description(
                    new HumanReadableDuration((int)taoWorkout["duration"]!).AsString(),
                    new HumanReadableDistance((double)taoWorkout["distance"]!, settings).AsString(),
                    "synced " + TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin") // TODO: Use Timezone here, too
                    ).ToString("ddd, HH:mm"),
                    "---"),
                new Phases(
                    new Yaapii.Atoms.Enumerable.Mapped<JsonNode, IEntity<IXocument>>(
                        x => new TAOJsonAsPhase(x, target.Memory(), settings).Value(),
                        taoWorkout["workoutSteps"]!.AsArray()!
                    )
                )
            );
            return target;
        })
        { }
    }
}
