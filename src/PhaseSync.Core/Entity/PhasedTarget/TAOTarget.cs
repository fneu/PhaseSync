using PhaseSync.Core.Entity.PhasedTarget.Facets;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class TAOTarget : EntityEnvelope<IHoneyComb>
    {
        public TAOTarget(IHive userHive, string taoWorkout) : base(() =>
        {
            var json = JsonObject.Parse(taoWorkout)!;
            var target = new PhasedTargetOf(userHive, (string)json["id"]!);
            var settings = new SettingsOf(userHive);

            var steps = json["workoutSteps"]!;
            target.Update(
                new Title((string)json["activitySubType"]!),
                new Time((string)json["start"]!, "Europe/Berlin"), // TODO: Get timezone from user settings
                new Description(
                    new HumanReadableDuration((int)json["duration"]!).AsString(),
                    new HumanReadableDistance((double)json["distance"]!, settings).AsString(),
                    "---"),
                new Phases(
                    new Yaapii.Atoms.Enumerable.Mapped<JsonNode, JsonNode>(
                        x => new Phase(x, settings).Value(),
                        json["workoutSteps"]!.AsArray()!
                    )
                )
            );
            return target;
        })
        { }
    }
}
