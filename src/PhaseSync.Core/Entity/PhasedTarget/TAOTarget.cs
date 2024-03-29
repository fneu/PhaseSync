﻿using PhaseSync.Core.Entity.Phase;
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

        public TAOTarget(IHive userHive, JsonNode taoWorkout) : this(
            userHive,
            taoWorkout,
            new SettingsOf(userHive)
        )
        { }

        public TAOTarget(IHive userHive, string taoWorkout, IEntity<IProps> settings) : this(
            userHive,
            JsonNode.Parse(taoWorkout)!,
            settings
        )
        { }

        public TAOTarget(IHive userHive, JsonNode taoWorkout, IEntity<IProps> settings) : base(() =>
        {
            var target = new PhasedTargetOf(userHive, ((string)taoWorkout["id"]!).Replace("/", "_"));

            var steps = taoWorkout["workoutSteps"]!;
            target.Update(
                new Title((string)taoWorkout["activitySubType"]!),
                new Time((string)taoWorkout["start"]!, "Europe/Berlin"), // TODO: Get timezone from user settings
                new ExpectedDistanceM((double)taoWorkout["distance"]!),
                new ExpectedDurationS((int)taoWorkout["duration"]!),
                new SyncedAt(
                    TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin") // TODO: Use Timezone here, too
                    ).ToString("HH:mm")
                ),
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
