﻿using PhaseSync.Core.Entity.Phase.Input;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class OpenManualPhase : EntityEnvelope<IXocument>
    {
        public OpenManualPhase(JsonNode workoutStep, IHoneyComb comb) : base(
            () =>
            {
                var phase = new PhaseOf(comb);
                phase.Update(
                    new Duration((int)(workoutStep["duration"] ?? 60)),
                    new Velocity((double)(workoutStep["velocity"] ?? 2.0)),
                    new Name((string)workoutStep["workoutStepType"]!)
                    );
                return phase;
            }
        )
        { }
    }
}
